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
    public class GroupService : IGroupService
    {

        private readonly IGroupRepository _groupRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ILogger<GroupService> _logger;

        public GroupService(IGroupRepository groupRepository, ITeamRepository teamRepository, ILogger<GroupService> logger)
        {
            _groupRepository = groupRepository;
            _teamRepository = teamRepository;
            _logger = logger;
        }

        public async Task SaveGroupsAsync(IEnumerable<Group> newGroups, int categoryId)
        {
            var validGroups = newGroups.Where(g => g.Teams.Any()).ToList();
            if (!validGroups.Any())
            {
                _logger.LogWarning("No valid groups to save for category {CategoryId}", categoryId);
                return;
            }

            _logger.LogInformation("Fetching teams from database for category {CategoryId}", categoryId);
            var teamsFromDb = await _teamRepository.GetTeamsByCategoryAsync(categoryId);
            var teamDictionary = teamsFromDb.ToDictionary(t => t.Id);

            foreach (var group in validGroups)
            {
                group.CategoryId = categoryId;

                // Ověříme, že všechny týmy ve skupinách existují v DB
                group.Teams = group.Teams
                    .Where(t => teamDictionary.ContainsKey(t.Id))
                    .Select(t => teamDictionary[t.Id])
                    .ToList();
            }

            _logger.LogInformation("Saving {Count} groups for category {CategoryId}", validGroups.Count, categoryId);
            await _groupRepository.SaveGroupsAsync(validGroups, categoryId);
            _logger.LogInformation("Groups saved successfully for category {CategoryId}", categoryId);
        }

        public async Task<IEnumerable<Group>> GetGroupsByCategoryAsync(int categoryId)
        {
            return await _groupRepository.GetGroupsByCategoryAsync(categoryId);
        }
    }
}
