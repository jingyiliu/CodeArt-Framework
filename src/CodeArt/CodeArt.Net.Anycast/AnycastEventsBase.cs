using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.Net.Anycast
{
    public class AnycastEventsBase
    {
        /// <summary>
        /// ��Ҫ2������
        /// 1) sender;
        /// 2) EventArgs ea;
        /// </summary>
        internal delegate void RaiseEvent(object[] args);

  
        #region �����쳣ʱ����

        public class ErrorEventArgs : EventArgs
        {
            public Exception Exception
            {
                get;
                private set;
            }
            
            public ErrorEventArgs(Exception exception)
            {
                this.Exception = exception;
            }
        }

        public delegate void ErrorEventHandler(object sender, ErrorEventArgs ea);

        public static event ErrorEventHandler Error;

        private static void _RaiseError(object[] args)
        {
            FireEvent(Error, args);
        }

        /// <summary>
        /// �첽�����ͻ��������ӵ��¼�
        /// </summary>
        /// <param name="data"></param>
        internal static void AsyncRaiseError(object sender, Exception ex)
        {
            object[] args = { sender, new ErrorEventArgs(ex) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseError), args);
        }


        #endregion

      
        /// <summary>
        /// �÷�������ʵ�ʴ����¼�
        /// ����ɹ�����ί����ô����true������false(�¼�û�б�����)
        /// </summary>
        /// <param name="del"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static bool FireEvent(Delegate del, object[] args)
        {
            bool result = false;

            if (del != null)
            {
                Delegate[] sinks = del.GetInvocationList();

                foreach (Delegate sink in sinks)
                {
                    try
                    {
                        sink.DynamicInvoke(args);
                    }
                    catch (Exception e)
                    {
                        AsyncRaiseError(args[0], e);
                    }
                }
                result = true;
            }

            return result;
        }
    }
}
