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

        public GroupRepository(ApplicationDbContext context, ILogger<GroupRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Group>> GetGroupsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Fetching groups for category {CategoryId}", categoryId);

            var groups = await _context.Groups
                .Include(g => g.Teams)
                .Include(t => t.Category)
                .Where(g => g.CategoryId == categoryId)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} groups for category {CategoryId}", groups.Count, categoryId);
            return groups;

        }

        public async Task SaveGroupsAsync(IEnumerable<Group> newGroups, int categoryId)
        {
            _logger.LogInformation("Starting transaction to save {Count} groups for category {CategoryId}", newGroups.Count(), categoryId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var groupsToDelete = await _context.Groups
                    .Where(g => g.CategoryId == categoryId)
                    .ToListAsync();

                if (groupsToDelete.Any())
                {
                    _context.Groups.RemoveRange(groupsToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("{Count} groups deleted for category {CategoryId}", groupsToDelete.Count, categoryId);
                }
                else
                {
                    _logger.LogWarning("No groups found to delete for category {CategoryId}", categoryId);
                }

                var validGroups = newGroups
                    .Where(g => g.Teams.Any())
                    .Select(g => new Group
                    {
                        Id = 0, 
                        Name = g.Name,
                        CategoryId = categoryId,
                        Teams = g.Teams 
                    })
                    .ToList();

                if (!validGroups.Any())
                {
                    _logger.LogWarning("No valid groups to save for category {CategoryId}, rolling back transaction", categoryId);
                    await transaction.RollbackAsync();
                    return;
                }

                _logger.LogInformation("Saving {Count} groups to database", validGroups.Count());
                await _context.Groups.AddRangeAsync(validGroups);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully saved {Count} groups for category {CategoryId}", validGroups.Count(), categoryId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error while saving groups. Transaction rolled back.");
                throw;
            }
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            _logger.LogInformation("Fetching all groups");
            var groups = await _context.Groups
                .Include(g => g.Teams)
                .Include(g => g.Category)
                .ToListAsync();
            _logger.LogInformation("Fetched {Count} groups", groups.Count);
            return groups;
        }

    }
}
