using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.Net.Anycast
{
    public class ServerEvents : AnycastEventsBase
    {
        #region �ͻ���������ʱ����

        public class ClientConnectedEventArgs : EventArgs
        {
            public IServerSession Session
            {
                get;
                private set;
            }

            public ClientConnectedEventArgs(IServerSession session)
            {
                this.Session = session;
            }
        }

        public delegate void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs ea);

        public static event ClientConnectedEventHandler ClientConnected;

        private static void _RaiseClientConnected(object[] args)
        {
            FireEvent(ClientConnected, args);
        }

        /// <summary>
        /// �첽�����ͻ��������ӵ��¼�
        /// </summary>
        /// <param name="data"></param>
        internal static void AsyncRaiseClientConnected(object sender, IServerSession session)
        {
            object[] args = { sender, new ClientConnectedEventArgs(session) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseClientConnected), args);
        }

        #endregion


        #region �ͻ��˶Ͽ�����ʱ����

        public class ClientDisconnectedEventArgs : EventArgs
        {
            public IServerSession Session
            {
                get;
                private set;
            }

            public ClientDisconnectedEventArgs(IServerSession session)
            {
                this.Session = session;
            }
        }

        public delegate void ClientDisconnectedEventHandler(object sender, ClientDisconnectedEventArgs ea);

        public static event ClientDisconnectedEventHandler ClientDisconnected;

        private static void _RaiseClientDisconnectedEvent(object[] args)
        {
            FireEvent(ClientDisconnected, args);
        }

        internal static void AsyncRaiseClientDisconnectedEvent(object sender, IServerSession session)
        {
            object[] args = { sender, new ClientDisconnectedEventArgs(session) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseClientDisconnectedEvent), args);
        }

        #endregion


        #region �ͻ���������ʱ����

        public class RunningEventArgs : EventArgs
        {
            public AnycastServer Server
            {
                get;
                private set;
            }

            public RunningEventArgs(AnycastServer server)
            {
                this.Server = server;
            }
        }

        public delegate void RunningEventHandler(object sender, RunningEventArgs ea);

        /// <summary>
        /// �������ѳɹ����е��¼�
        /// </summary>
        public static event RunningEventHandler Running;

        private static void _RaiseRunning(object[] args)
        {
            FireEvent(Running, args);
        }

        internal static void AsyncRaiseRunning(object sender, AnycastServer server)
        {
            object[] args = { sender, new RunningEventArgs(server) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseRunning), args);
        }

        #endregion


        #region �����¼�

        /// <summary>
        /// �յ�������
        /// </summary>
        public class HeartBeatReceivedEventArgs : EventArgs
        {
            public IServerSession Session
            {
                get;
                private set;
            }

            public HeartBeatReceivedEventArgs(IServerSession session)
            {
                this.Session = session;
            }
        }

        public delegate void HeartBeatReceivedEventHandler(object sender, HeartBeatReceivedEventArgs ea);

        public static event HeartBeatReceivedEventHandler HeartBeatReceived;

        private static void _RaiseHeartBeatReceived(object[] args)
        {
            FireEvent(HeartBeatReceived, args);
        }

        internal static void AsyncRaiseHeartBeatReceived(object sender, IServerSession session)
        {
            if (HeartBeatReceived == null) return;

            object[] args = { sender, new HeartBeatReceivedEventArgs(session) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseHeartBeatReceived), args);
        }

        #endregion

    }
}
