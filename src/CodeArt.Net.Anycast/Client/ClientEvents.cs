using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.Net.Anycast
{
    public class ClientEvents : AnycastEventsBase
    {
        #region �������ӷ�����

        /// <summary>
        /// �������ӷ��������¼�
        /// </summary>
        public class ConnectingEventArgs : EventArgs
        {
            /// <summary>
            /// �Ƿ�Ϊ��������
            /// </summary>
            public bool IsReconnect
            {
                get
                {
                    return this.ReconnectArgs != null && this.ReconnectArgs.Times > 0;
                }
            }

            public ReconnectArgs ReconnectArgs
            {
                get;
                private set;
            }


            internal ConnectingEventArgs(ReconnectArgs reconnectArgs)
            {
                this.ReconnectArgs = reconnectArgs;
            }
        }

        public delegate void ConnectingEventHandler(object sender, ConnectingEventArgs ea);

        public static event ConnectingEventHandler Connecting;

        private static void _RaiseConnecting(object[] args)
        {
            FireEvent(Connecting, args);
        }

        internal static void AsyncRaiseConnecting(object sender, ReconnectArgs reconnectArgs)
        {
            if (Connecting == null) return;

            object[] args = { sender, new ConnectingEventArgs(reconnectArgs) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseConnecting), args);
        }


        #endregion

        #region ���ӷ������ɹ�

        /// <summary>
        /// ���ӷ������ɹ����¼�
        /// </summary>
        public class ConnectedEventArgs : EventArgs
        {
            public IPEndPoint ServerEndPoint
            {
                get;
                private set;
            }

            public ConnectedEventArgs(IPEndPoint serverEndPoint)
            {
                this.ServerEndPoint = serverEndPoint;
            }
        }

        public delegate void ConnectedEventHandler(object sender, ConnectedEventArgs ea);

        public static event ConnectedEventHandler Connected;

        private static void _RaiseConnected(object[] args)
        {
            FireEvent(Connected, args);
        }

        internal static void AsyncRaiseConnected(object sender, IPEndPoint serverEndPoint)
        {
            if (Connected == null) return;

            object[] args = { sender, new ConnectedEventArgs(serverEndPoint) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseConnected), args);
        }

        #endregion

        #region �����¼�

        /// <summary>
        /// �յ�������
        /// </summary>
        public class HeartBeatReceivedEventArgs : EventArgs
        {
            public IPEndPoint ServerEndPoint
            {
                get;
                private set;
            }

            public HeartBeatReceivedEventArgs(IPEndPoint serverEndPoint)
            {
                this.ServerEndPoint = serverEndPoint;
            }
        }

        public delegate void HeartBeatReceivedEventHandler(object sender, HeartBeatReceivedEventArgs ea);

        public static event HeartBeatReceivedEventHandler HeartBeatReceived;

        private static void _RaiseHeartBeatReceived(object[] args)
        {
            FireEvent(HeartBeatReceived, args);
        }

        internal static void AsyncRaiseHeartBeatReceived(object sender, IPEndPoint serverEndPoint)
        {
            if (HeartBeatReceived == null) return;

            object[] args = { sender, new HeartBeatReceivedEventArgs(serverEndPoint) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseHeartBeatReceived), args);
        }

        #endregion

        #region �ӷ������Ͽ����¼�

        /// <summary>
        /// �ӷ������Ͽ����¼��������ǵ��ߡ����������˳����ᴥ���Ͽ����ӵ��¼�
        /// </summary>
        public class DisconnectedEventArgs : EventArgs
        {
            /// <summary>
            /// �Ƿ���Ϊ���粻ͨ�������쳣��ԭ���µ����ӶϿ�
            /// </summary>
            public bool IsDropped
            {
                get;
                private set;
            }

            public DisconnectedEventArgs(bool isDropped)
            {
                this.IsDropped = isDropped;
            }
        }

        public delegate void DisconnectedEventHandler(object sender, DisconnectedEventArgs ea);

        public static event DisconnectedEventHandler Disconnected;

        private static void _RaiseDisconnected(object[] args)
        {
            FireEvent(Disconnected, args);
        }

        internal static void AsyncRaiseDisconnected(object sender, bool isDropped)
        {
            if (Disconnected == null) return;

            object[] args = { sender, new DisconnectedEventArgs(isDropped) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseDisconnected), args);
        }

        #endregion

        #region ��γ���������������ʧ��

        /// <summary>
        /// ��γ���������������ʧ��
        /// </summary>
        public class ReconnectFailedEventArgs : EventArgs
        {
            public ReconnectFailedEventArgs()
            {
            }
        }

        public delegate void ReconnectFailedEventHandler(object sender, ReconnectFailedEventArgs ea);

        /// <summary>
        /// ��γ���������������ʧ��
        /// </summary>
        public static event ReconnectFailedEventHandler ReconnectFailed;

        private static void _ReconnectFailed(object[] args)
        {
            FireEvent(ReconnectFailed, args);
        }

        internal static void AsyncRaiseReconnectFailed(object sender)
        {
            if (ReconnectFailed == null) return;

            object[] args = { sender, new ReconnectFailedEventArgs() };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_ReconnectFailed), args);
        }

        #endregion

        /// <summary>
        /// �鲥�����˵��¼�����
        /// </summary>
        public class ParticipantEventArgs : EventArgs
        {
            public Multicast Multicast
            {
                get;
                private set;
            }

            public Participant Participant
            {
                get;
                private set;
            }

            /// <summary>
            /// �������Ƿ�Ϊ���ص�
            /// </summary>
            public bool IsLocal
            {
                get;
                private set;
            }

            public ParticipantEventArgs(Multicast multicast, Participant participant,bool isLocal)
            {
                this.Multicast = multicast;
                this.Participant = participant;
                this.IsLocal = isLocal;
            }
        }


        #region �µ��鲥��Ա������¼�

        public delegate void ParticipantAddedEventHandler(object sender, ParticipantEventArgs ea);

        public static event ParticipantAddedEventHandler ParticipantAdded;

        private static void _RaiseParticipantAdded(object[] args)
        {
            FireEvent(ParticipantAdded, args);
        }

        internal static void AsyncRaiseParticipantAdded(object sender, Multicast multicast, Participant participant,bool isLocal)
        {
            if (ParticipantAdded == null) return;

            object[] args = { sender, new ParticipantEventArgs(multicast, participant, isLocal) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseParticipantAdded), args);
        }

        #endregion

        #region �鲥��Ա���ݱ����ĵ��¼�

        public delegate void ParticipantUpdatedEventHandler(object sender, ParticipantEventArgs ea);

        public static event ParticipantUpdatedEventHandler ParticipantUpdated;

        private static void _RaiseParticipantUpdated(object[] args)
        {
            FireEvent(ParticipantUpdated, args);
        }

        internal static void AsyncRaiseParticipantUpdated(object sender, Multicast multicast, Participant participant, bool isLocal)
        {
            if (ParticipantUpdated == null) return;

            object[] args = { sender, new ParticipantEventArgs(multicast, participant,isLocal) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseParticipantUpdated), args);
        }

        #endregion

        #region �鲥��Ա�뿪���¼�

        public delegate void ParticipantRemovedEventHandler(object sender, ParticipantEventArgs ea);


        public static event ParticipantRemovedEventHandler ParticipantRemoved;

        private static void _RaiseParticipantRemoved(object[] args)
        {
            FireEvent(ParticipantRemoved, args);
        }

        internal static void AsyncRaiseParticipantRemoved(object sender, Multicast multicast, Participant participant, bool isLocal)
        {
            if (ParticipantRemoved == null) return;

            object[] args = { sender, new ParticipantEventArgs(multicast, participant, isLocal) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseParticipantRemoved), args);
        }

        #endregion

        #region �յ���rtp����ʱ����

        public class MessageReceivedEventArgs : EventArgs
        {
            public Message Message
            {
                get;
                private set;
            }

            /// <summary>
            /// ��Ϣ���͵�Ŀ�ĵ�ַ
            /// </summary>
            public string Destination
            {
                get;
                private set;
            }


            public MessageReceivedEventArgs(Message message)
            {
                this.Message = message;
                this.Destination = message.Header.GetValue<string>(MessageField.Destination, string.Empty);
            }
        }

        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs ea);

        internal static event MessageReceivedEventHandler MessageReceived;

        private static void _RaiseMessageReceived(object[] args)
        {
            FireEvent(MessageReceived, args);
        }

        internal static void AsyncRaiseMessageReceived(object sender, Message msg)
        {
            if (MessageReceived == null) return;

            object[] args = { sender, new MessageReceivedEventArgs(msg) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseMessageReceived), args);
        }

        #endregion

        //#region �¼�����

        ///// <summary>
        ///// �鲥�����˵��¼�����
        ///// </summary>
        //public class RtpParticipantEventArgs : EventArgs
        //{
        //    public RtpParticipant RtpParticipant
        //    {
        //        get;
        //        private set;
        //    }

        //    public RtpMulticast Multicast
        //    {
        //        get;
        //        private set;
        //    }

        //    public RtpParticipantEventArgs(RtpMulticast multicast, RtpParticipant participant)
        //    {
        //        this.Multicast = multicast;
        //        this.RtpParticipant = participant;
        //    }
        //}

        //#endregion

        //#region ����ת������ʱ�Ļص�

        //public class RtpUnicastCallbackEventArgs : EventArgs
        //{
        //    public string UnicastAddress
        //    {
        //        get;
        //        private set;
        //    }


        //    public string EventSource
        //    {
        //        get;
        //        private set;
        //    }

        //    public bool Success
        //    {
        //        get;
        //        private set;
        //    }


        //    public RtpUnicastCallbackEventArgs(string unicastAddress, string eventSource, bool success)
        //    {
        //        this.UnicastAddress = unicastAddress;
        //        this.EventSource = eventSource;
        //        this.Success = success;
        //    }
        //}

        //public delegate void RtpUnicastCallbackEventHandler(object sender, RtpUnicastCallbackEventArgs ea);

        //public static event RtpUnicastCallbackEventHandler RtpUnicastCallback;

        //internal static void RaiseRtpUnicastCallbackEvent(object[] args)
        //{
        //    FireEvent(RtpUnicastCallback, args);
        //}

        //#endregion


        //#region ����¼�

        ///// <summary>
        ///// �յ������ص����ݰ�
        ///// </summary>
        //public class RtpDataPackagePluginReceivedEventArgs : EventArgs
        //{
        //    public RtpDataPackage DataPackage
        //    {
        //        get;
        //        private set;
        //    }

        //    public string PluginName
        //    {
        //        get
        //        {
        //            return this.DataPackage.PluginName;
        //        }
        //    }

        //    public string EventSource
        //    {
        //        get
        //        {
        //            return this.DataPackage.EventSource;
        //        }
        //    }


        //    public RtpDataPackagePluginReceivedEventArgs(RtpDataPackage package)
        //    {
        //        this.DataPackage = package;
        //    }
        //}

        //public delegate void RtpDataPackagePluginReceivedEventHandler(object sender, RtpDataPackagePluginReceivedEventArgs ea);

        //public static event RtpDataPackagePluginReceivedEventHandler RtpDataPackagePluginReceived;

        //internal static void RaiseRtpDataPackagePluginReceivedEvent(object[] args)
        //{
        //    FireEvent(RtpDataPackagePluginReceived, args);
        //}


        //#endregion
    }
}
