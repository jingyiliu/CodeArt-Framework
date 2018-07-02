using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.DTO;

namespace CodeArt.EasyMQ.Event
{
    /// <summary>
    /// Զ���¼�������
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="arg"></param>
        void Handle(string eventName, DTObject arg);
    }
}