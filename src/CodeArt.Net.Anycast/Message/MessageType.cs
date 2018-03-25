using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;

using CodeArt.DTO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace CodeArt.Net.Anycast
{
    public enum MessageType : byte
    {
        Unspecified = 0,

        /// <summary>
        /// ��¼����
        /// </summary>
        LoginRequest = 1,
        /// <summary>
        /// ��¼��Ӧ
        /// </summary>
        LoginResponse = 2,

        /// <summary>
        /// ��������
        /// </summary>
        HeartBeatRequest = 3,

        /// <summary>
        /// ������Ӧ
        /// </summary>
        HeartBeatResponse = 4,

        /// <summary>
        /// �����鲥
        /// </summary>
        Join = 5,

        /// <summary>
        /// �뿪�鲥
        /// </summary>
        Leave = 6,

        /// <summary>
        /// ת����Ϣ
        /// </summary>
        Distribute = 7,

        /// <summary>
        /// ����һ�� i'm here ��Ϣ����ʾ���ͷ����鲥��
        /// </summary>
        IAmHere = 8,

        /// <summary>
        /// ����һ�� i'm not hrer ��Ϣ����ʾ���ͷ������鲥��
        /// </summary>
        IAmNotHere = 9,

        /// <summary>
        /// �����˱��ı����Ϣ
        /// </summary>
        ParticipantUpdated = 10,

        /// <summary>
        /// �Զ���
        /// </summary>
        Custom = 99
    }
}
