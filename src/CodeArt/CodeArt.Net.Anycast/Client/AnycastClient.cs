using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Threading;

using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Handlers.Timeout;

using CodeArt.DTO;
using CodeArt.Concurrent.Pattern;
using CodeArt.Concurrent;
using DotNetty.Buffers;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// �β��ͻ���
    /// </summary>
    public class AnycastClient : IDisposable
    {
        public ClientConfig Config
        {
            get;
            private set;
        }

        public IPEndPoint ServerEndPoint
        {
            get;
            private set;
        }

        public IChannel Channel
        {
            get;
            internal set;
        }

        private IEventLoopGroup _group;

        internal ConnectionStatus Status
        {
            get;
            set;
        }

        public bool IsActive
        {
            get
            {
                return Channel != null && Channel.Active && this.Status == ConnectionStatus.Connected;
            }
        }

        /// <summary>
        /// �ͻ������ڵĵ�ַ
        /// </summary>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// �������ͬһ���̴߳����Ļ���������ô�������ǲ������õģ�����
        /// </summary>
        private ActionPipeline _pipeline;

        public AnycastClient(ClientConfig config)
        {
            this.Config = config;
            this.Status = ConnectionStatus.Disconnected;

            _listeners = new List<IMessageListener>();

            _pipeline = new ActionPipeline();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tryReconnect">������ʧ�ܺ��Ƿ������������ò���Ӱ����������������</param>
        /// <returns></returns>
        public void Connect(bool tryReconnect = false)
        {
            _pipeline.Queue(() =>
            {
                if (tryReconnect)
                {
                    _Connect(new ReconnectArgs());
                }
                else
                {
                    _Connect(null);
                }
            });
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        private void _Connect(ReconnectArgs reconnectArgs)
        {
            if (_disposed) return;
            if (this.Status != ConnectionStatus.Disconnected) return;
            this.Status = ConnectionStatus.Connecting;

            if(_group == null)
                _group = new MultithreadEventLoopGroup();

            X509Certificate2 cert = null;
            string targetHost = null;
            if (this.Config.IsSsl)
            {
                cert = new X509Certificate2(Path.Combine(AppContext.ProcessDirectory, "anycast.pfx"), "password");
                targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            }

            var bootstrap = new Bootstrap();
            bootstrap
                .Group(_group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default) //���ڶ����������ڴ�й¶���⣬��������ʹ�� �ǳص��ֽڻ�����
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    if (cert != null)
                    {
                        pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                    }
                    pipeline.AddLast(new LoggingHandler());
                    pipeline.AddLast("framing-enc", new MessageEncoder());
                    pipeline.AddLast("framing-dec", new MessageDecoder());

                    pipeline.AddLast("ReadTimeout", new ReadTimeoutHandler(Settings.Timeout)); //����ָ��ʱ��û�н��յ��κ���Ϣ���Զ��ر�
                    pipeline.AddLast("LoginAuthReq", new LoginAuthReqHandler(this));
                    pipeline.AddLast("HeartBeatReq", new HeartBeatReqHandler(this));
                    pipeline.AddLast("ClientLogic", new ClientLogicHandler(this));
                }));

            this.ServerEndPoint = new IPEndPoint(IPAddress.Parse(this.Config.Host), this.Config.Port);
            ClientEvents.AsyncRaiseConnecting(this, reconnectArgs);

            try
            {
                if (reconnectArgs != null && reconnectArgs.Times > 0)  //��0��������ζ����ֱ�����ӣ����õȴ�
                {
                    _reconnectCancellation = new CancellationTokenSource();
                    Task.Delay(TimeSpan.FromSeconds(this.Config.ReconnectDelayTime), _reconnectCancellation.Token).Wait();
                }

                this.Channel = bootstrap.ConnectAsync(this.ServerEndPoint).Result;
                this.Address = this.Channel.LocalAddress.ToString();  //��client�����ǵĵ�ַ��ͨ���ı��ص�ַ

                //while (true)
                //{
                //    this.Channel = bootstrap.ConnectAsync(this.ServerEndPoint).Result;
                //    this.Address = this.Channel.LocalAddress.ToString();  //��client�����ǵĵ�ַ��ͨ���ı��ص�ַ
                //    System.Threading.Thread.Sleep(2000);
                //    this.Channel.CloseAsync().Wait();
                //}


            }
            catch (Exception ex)
            {
                this.Status = ConnectionStatus.Disconnected;
                var args = reconnectArgs?.Clone();
                ClientEvents.AsyncRaiseError(this, new ConnectServerException(this.ServerEndPoint, ex, args));
                if (reconnectArgs != null)
                {
                    _pipeline.Queue(()=>
                    {
                        _Reconnect(reconnectArgs);
                    });
                }
            }
        }

        /// <summary>
        /// �ͻ����ѵ���
        /// </summary>
        internal void Dropped()
        {
            _pipeline.Queue(() =>
            {
                _Disconnect(true);
            });
        }

        private void _Disconnect(bool isDropped)
        {
            if (_disposed) return;
            if (this.Status != ConnectionStatus.Connected) return;
            this.Status = ConnectionStatus.Disconnected;

            try
            {
                DisposeReconnectCancellation();

                if (Channel != null)
                {
                    this.LeaveAll();
                    _DisposeChannel();
                    ClientEvents.AsyncRaiseDisconnected(this, isDropped);
                }
            }
            catch(Exception ex)
            {
                ClientEvents.AsyncRaiseError(this, ex);
            }
            finally
            {
                if (isDropped)
                {
                    _pipeline.Queue(() =>
                    {
                        _Reconnect(new ReconnectArgs());
                    });
                }
            }
        }

        private CancellationTokenSource _reconnectCancellation;

        private void DisposeReconnectCancellation()
        {
            if (_reconnectCancellation != null)
            {
                _reconnectCancellation.Cancel(true);
                _reconnectCancellation.Dispose();
                _reconnectCancellation = null;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        private void _Reconnect(ReconnectArgs arg)
        {
            DisposeReconnectCancellation();

            if (this.Config.ReconnectTimes > 0 && arg.Times == this.Config.ReconnectTimes)
            {
                //this.Config.ReconnectTimesС�ڻ��ߵ���0��ʱ�򣬲�����������Ϊ��������
                ClientEvents.AsyncRaiseError(this, new ReconnectFailedException());
                return;
            }

            arg.Times++;
            _Connect(arg);
        }

        private void _DisposeChannel()
        {
            if (this.Channel != null)
            {
                this.Channel.DisconnectAsync().Wait();
                this.Channel.CloseAsync().Wait();
                this.Address = null;
                this.Channel = null;
            }
        }

        private bool _disposed = false;

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public void Dispose()
        {
            _pipeline.Queue(() =>
            {
                if (_disposed) return;
                _Disconnect(false);
                _DisposeChannel(); //��ֹ����״̬����ͨ������δ���ͷţ����������ٴ��ͷ�һ��
                if (_group != null)
                {
                    _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)).Wait();
                    _group = null;
                }
                _disposed = true;
                _pipeline.Dispose();
            });
        }


        #region ��Ϣ����

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="message"></param>
        public void Process(Message message)
        {
            foreach (var listener in _listeners)
                listener.Process(this, message);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="message"></param>
        public Future<bool> Send(Message message)
        {
            var future = new Future<bool>();
            future.Start();
            _pipeline.Queue(() =>
            {
                if (this.Channel == null)
                    future.SetResult(false);
                else
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            var task = Channel?.WriteAndFlushAsync(message);
                            task.Wait();
                            future.SetResult(true);
                        }
                        catch (Exception ex)
                        {
                            future.SetError(ex);
                        }
                    });
                }
            });
            return future;
        }

        #endregion

        private List<IMessageListener> _listeners;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public AnycastClient AddListener(IMessageListener listener)
        {
            _listeners.Add(listener);
            return this;
        }


        #region �鲥��ַ

        private List<Multicast> _multicasts = new List<Multicast>();

        /// <summary>
        /// ���ڲ����Ե�ԭ�����ǽ��ṩUse�����������ṩGet����ֱ�ӷ����鲥����
        /// </summary>
        /// <param name="multicastAddress"></param>
        /// <param name="action"></param>
        public void UseMulticast(string multicastAddress,Action<Multicast> action)
        {
            lock (_multicasts)
            {
                var multicast = GetMulticast(multicastAddress);
                if (multicast == null) return;
                action(multicast);
            }
        }


        private Multicast GetMulticast(string multicastAddress)
        {
            lock (_multicasts)
            {
                return _multicasts.FirstOrDefault((t) =>
                {
                    return t.Address == multicastAddress;
                });
            }
        }


        /// <summary>
        /// �ûػ����뵽���鲥��ַ��
        /// </summary>
        public Multicast[] Multicasts
        {
            get
            {
                lock (_multicasts)
                {
                    return _multicasts.ToArray();
                }
            }
        }

        public Multicast Join(string multicastAddress, Participant participant)
        {
            var multicast = GetMulticast(multicastAddress);
            if (multicast != null) return multicast;

            lock (_multicasts)
            {
                multicast = GetMulticast(multicastAddress);
                if (multicast != null) return multicast;

                multicast = new Multicast(this, multicastAddress, participant);
                multicast.Join();
                _multicasts.Add(multicast);
            }
            return multicast;
        }

        public void Leave(string multicastAddress)
        {
            var multicast = GetMulticast(multicastAddress);
            if (multicast == null) return;

            lock (_multicasts)
            {
                multicast = GetMulticast(multicastAddress);
                if (multicast == null) return;

                multicast.Leave();
                _multicasts.Remove(multicast);
            }
        }

        /// <summary>
        /// �뿪�����鲥
        /// </summary>
        private void LeaveAll()
        {
            var multicasts = this.Multicasts;
            foreach (var multicast in multicasts)
            {
                Leave(multicast.Address);
            }
        }

        #endregion

        #region  ������

        public Participant[] GetParticipants(string multicastAddress)
        {
            var multicast = GetMulticast(multicastAddress);
            return multicast == null ? Array.Empty<Participant>() : multicast.Participants;
        }

        public Participant GetParticipant(string multicastAddress, string participantId)
        {
            var multicast = GetMulticast(multicastAddress);
            return multicast == null ? null : multicast.GetParticipant(participantId);
        }

        public void UpdateParticipant(string multicastAddress, Participant participant)
        {
            var multicast = GetMulticast(multicastAddress);
            if (multicast == null) return;

            lock (_multicasts)
            {
                multicast = GetMulticast(multicastAddress);
                if (multicast == null) return;
                multicast.UpdateHost(participant);
            }
        }


        #endregion



    }
}