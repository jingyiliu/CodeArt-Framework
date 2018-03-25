﻿using System;

using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    public class NotFoundObjectException : DomainDrivenException
    {
        public NotFoundObjectException(Type objectType, object id)
            : base(string.Format(Strings.NoObjectById, id, objectType.FullName))
        {
        }
    }
}
