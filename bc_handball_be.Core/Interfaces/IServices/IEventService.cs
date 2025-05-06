using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface IEventService
    {
        Task AddEventAsync(Event newEvent);
        Task<List<Event>> GetEventsByMatchIdAsync(int matchId);
        Task DeleteAllByMatchIdAsync(int matchId);
        Task<Event?> DeleteLastNonInfoEventAsync(int matchId);
    }
}
