using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task AddEventAsync(Event newEvent)
        {
            try
            {
                await _eventRepository.AddAsync(newEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding event");
                throw;
            }
        }

        public async Task<List<Event>> GetEventsByMatchIdAsync(int matchId)
        {
            return await _eventRepository.GetEventsByMatchIdAsync(matchId);
        }

        public async Task DeleteAllByMatchIdAsync(int matchId)
        {
            try
            {
                var events = await _eventRepository.GetEventsByMatchIdAsync(matchId);
                if (events != null && events.Any())
                {
                    foreach (var eventItem in events)
                    {
                        await _eventRepository.DeleteAsync(eventItem);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting events by match ID");
                throw;
            }
        }

        public async Task<Event?> DeleteLastNonInfoEventAsync(int matchId)
        {
            var events = await _eventRepository.GetEventsByMatchIdAsync(matchId);
            var last = events.LastOrDefault(e => e.Type != "I");
            if (last != null)
            {
                await _eventRepository.DeleteAsync(last);
            }
            return last;
        }
    }
}
