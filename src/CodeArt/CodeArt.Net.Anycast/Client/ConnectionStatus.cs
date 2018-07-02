using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// ����״̬
    /// </summary>
    public enum ConnectionStatus : byte
    {
        /// <summary>
        /// ��������
        /// </summary>
        Connecting = 1,
        /// <summary>
        /// ������
        /// </summary>
        Connected = 2,
        /// <summary>
        /// �ѶϿ�
        /// </summary>
        Disconnected = 3
    }
}
