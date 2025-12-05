using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GroupRepository> _logger;

        public GroupRepository(ApplicationDbContext context, ILogger<GroupRepository> _logger)
        {
            _context = context;
            this._logger = _logger;
        }

        // ==================== READ OPERATIONS ====================

        public async Task<IEnumerable<Group>> GetGroupsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Fetching groups for category {CategoryId}", categoryId);

            var groups = await _context.Groups
                .Include(g => g.TeamGroups)
                    .ThenInclude(tg => tg.Team)
                        .ThenInclude(t => t.Club)
                .Include(t => t.Category)
                .Where(g => g.CategoryId == categoryId)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} groups for category {CategoryId}", groups.Count, categoryId);
            return groups;
        }

        public async Task<List<Group>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all groups");
            var groups = await _context.Groups
                .Include(g => g.TeamGroups)
                    .ThenInclude(tg => tg.Team)
                        .ThenInclude(t => t.Club)
                .Include(g => g.Category)
                .ToListAsync();
            _logger.LogInformation("Fetched {Count} groups", groups.Count);
            return groups;
        }

        public async Task<Group?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching group with ID {GroupId}", id);
            return await _context.Groups
                .Include(g => g.TeamGroups)
                    .ThenInclude(tg => tg.Team)
                        .ThenInclude(t => t.Club)
                .Include(g => g.Category)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Group>> GetByPhaseAsync(int categoryId, string phase)
        {
            _logger.LogInformation("Fetching groups for category {CategoryId} with phase {Phase}", categoryId, phase);
            return await _context.Groups
                .Where(g => g.CategoryId == categoryId && g.Phase == phase)
                .ToListAsync();
        }

        public async Task<List<Group>> GetGroupsWithPlaceholderTeamsAsync(int categoryId)
        {
            _logger.LogInformation("Fetching groups with placeholder teams for category {CategoryId}", categoryId);
            var groups = await _context.Groups
                .Include(g => g.TeamGroups)
                    .ThenInclude(tg => tg.Team)
                .Include(g => g.Category)
                .Where(g => g.CategoryId == categoryId &&
                    g.TeamGroups.Any(tg => tg.Team.IsPlaceholder == true))
                .ToListAsync();
            return groups;
        }

        // ==================== WRITE OPERATIONS ====================

        public async Task AddAsync(Group group)
        {
            _logger.LogInformation("Adding group {GroupName} for category {CategoryId}", group.Name, group.CategoryId);
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Group> groups)
        {
            _logger.LogInformation("Adding {Count} groups to database", groups.Count());
            await _context.Groups.AddRangeAsync(groups);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Group group)
        {
            _logger.LogInformation("Updating group {GroupId}", group.Id);
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        // ==================== DELETE OPERATIONS ====================

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting group with ID {GroupId}", id);
            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Group with ID {GroupId} not found for deletion", id);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<Group> groups)
        {
            _logger.LogInformation("Deleting {Count} groups", groups.Count());
            _context.Groups.RemoveRange(groups);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByCategoryIdAsync(int categoryId)
        {
            _logger.LogInformation("Deleting groups for category {CategoryId}", categoryId);
            var groups = await _context.Groups
                .Where(g => g.CategoryId == categoryId)
                .ToListAsync();
            if (groups.Any())
            {
                _context.Groups.RemoveRange(groups);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted {Count} groups for category {CategoryId}", groups.Count, categoryId);
            }
            else
            {
                _logger.LogWarning("No groups found to delete for category {CategoryId}", categoryId);
            }
        }
    }
}
