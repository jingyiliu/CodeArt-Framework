﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public interface IEventQueueRepository : IRepository<EventQueue>
    {
        EventQueue FindByEventId(Guid eventId, QueryLevel level);
    }
}
