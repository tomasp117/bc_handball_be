﻿using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface IEventRepository
    {
        Task AddAsync(Event newEvent);
        Task<List<Event>> GetEventsByMatchIdAsync(int matchId);
        Task DeleteAsync(Event eventItem);
    }
}
