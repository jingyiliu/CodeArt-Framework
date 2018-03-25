using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

using CodeArt.Concurrent.Pattern;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// ά��һ����̨�̣߳����ϵĴӶ�������ȡ����������й�����
    /// �¼������Ὣ��������뵽������
    /// </summary>
    internal class AnycastEventThrower
    {
        private static AutoResetPipeline _throwEvent;
        private static int _peakQueueLength = 0;
        private static ConcurrentQueue<WorkItem> _syncWorkItems = new ConcurrentQueue<WorkItem>();

        static public int WorkItemQueueLength
        {
            get
            {
                return _syncWorkItems.Count;
            }
        }
        static public int PeakEventQueueLength
        {
            get
            {
                return _peakQueueLength;
            }
        }

        static AnycastEventThrower()
        {
            _throwEvent = new AutoResetPipeline(EventThread);
        }


        private static void EventThread()
        {
            while (_syncWorkItems.Count > 0)
            {
                WorkItem wi = WorkItem.Empty;
                if (_syncWorkItems.TryDequeue(out wi))
                {
                    wi.method(wi.parameters);
                }
            }
        }



        internal static void QueueUserWorkItem(AnycastEventsBase.RaiseEvent del, object[] parameters)
        {
            _syncWorkItems.Enqueue(new WorkItem(del, parameters));

            if (_peakQueueLength < _syncWorkItems.Count)
                _peakQueueLength = _syncWorkItems.Count;

            _throwEvent.AllowOne();
        }

        /// <summary>
        /// ������
        /// </summary>
        private struct WorkItem
        {
            public AnycastEventsBase.RaiseEvent method;
            public object[] parameters;

            public WorkItem(AnycastEventsBase.RaiseEvent method, object[] parameters)
            {
                this.method = method;
                this.parameters = parameters;
            }

            public static readonly WorkItem Empty = new WorkItem(null, null);
        }

    }
}
