using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventRepository> _logger;
        public EventRepository(ApplicationDbContext context, ILogger<EventRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task AddAsync(Event newEvent)
        {
            try
            {
                _logger.LogInformation("Adding new event to the database");
                await _context.Events.AddAsync(newEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding event");
                throw;
            }

        }

        public async Task<List<Event>> GetEventsByMatchIdAsync(int matchId)
        {
            try
            {
                _logger.LogInformation("Fetching events for matchId: {MatchId}", matchId);
                var events = await _context.Events.Where(e => e.MatchId == matchId).ToListAsync();
                if (!events.Any())
                {
                    _logger.LogWarning("No events found for matchId: {MatchId}", matchId);
                }
                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching events for matchId: {MatchId}", matchId);
                throw;
            }
        }

        public async Task DeleteAsync(Event eventItem)
        {
            try
            {
                _logger.LogInformation("Deleting event with ID: {EventId}", eventItem.Id);
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event with ID: {EventId}", eventItem.Id);
                throw;
            }
        }
    }
}
