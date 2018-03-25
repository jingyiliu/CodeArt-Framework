﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 客户端身份提供者
    /// </summary>
    public sealed class EmptyIdentity : IIdentityProvider
    {
        private EmptyIdentity() { }

        public DTObject GetIdentity()
        {
            return DTObject.Empty;
        }

        public readonly static EmptyIdentity Instance = new EmptyIdentity();

    }
}
