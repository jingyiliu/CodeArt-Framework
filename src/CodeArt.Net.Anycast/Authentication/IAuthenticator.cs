﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.Net.Anycast
{
    public interface IAuthenticator
    {
        CertifiedResult Check(DTObject identity);
    }
}
