using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using bc_handball_be.Core.Services.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ILogger<TeamService> _logger;

        public TeamService(ITeamRepository teamRepository, ILogger<TeamService> logger)
        {
            _teamRepository = teamRepository;
            _logger = logger;
        }

        public Task AddTeamAsync(Team team)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GroupAssignmentVariant>> AssignTeamsToGroupsAsync(
            IEnumerable<TeamWithAttributes> teamsWithAttributes, int categoryId)
        {
            _logger.LogInformation("Starting team assignment for {TeamCount} teams", teamsWithAttributes.Count());

            var variants = new List<GroupAssignmentVariant>();

            for (int groupCount = 3; groupCount <= 5; groupCount++)
            {
                var groups = CreateBalancedGroups(teamsWithAttributes.ToList(), groupCount);
                int totalMatches = CalculateTotalMatches(groups);
                int minMatchesPerTeam = groups.Min(g => g.TeamGroups.Count > 1 ? (g.TeamGroups.Count - 1) : 0);

                variants.Add(new GroupAssignmentVariant
                {
                    GroupCount = groupCount,
                    TotalMatches = totalMatches,
                    MinMatchesPerTeam = minMatchesPerTeam,
                    Groups = groups
                });
            }

            return await Task.FromResult(variants);
        }

        private List<Group> CreateBalancedGroups(List<TeamWithAttributes> teams, int groupCount)
        {
            var sortedTeams = teams.OrderByDescending(t => t.Strength).ToList();

            var groups = Enumerable.Range(1, groupCount)
                .Select(i => new Group { Id = i - 1, Name = $"{Convert.ToChar('A' + i - 1)}", TeamGroups = new List<TeamGroup>() })
                .ToList();

            int direction = 1;
            int groupIndex = 0;

            foreach (var teamData in sortedTeams)
            {
                if (teamData.IsGirls)
                {
                    var availableGroups = groups.Where(g => !g.TeamGroups.Any(tg =>
                        teams.FirstOrDefault(attr => attr.Team.Id == tg.TeamId)?.IsGirls == true))
                        .ToList();

                    if (availableGroups.Any())
                    {
                        var selectedGroup = availableGroups.OrderBy(g => g.TeamGroups.Count).First();
                        selectedGroup.TeamGroups.Add(new TeamGroup
                        {
                            TeamId = teamData.Team.Id,
                            Team = teamData.Team,
                            Group = selectedGroup
                        });
                        continue;
                    }
                }

                var currentGroup = groups[groupIndex];
                currentGroup.TeamGroups.Add(new TeamGroup
                {
                    TeamId = teamData.Team.Id,
                    Team = teamData.Team,
                    Group = currentGroup
                });

                groupIndex += direction;

                if (groupIndex == groupCount || groupIndex == -1)
                {
                    direction *= -1;
                    groupIndex += direction;
                }
            }

            return groups;
        }



        private int CalculateTotalMatches(IEnumerable<Group> groups)
        {
            return groups.Sum(group => (group.TeamGroups.Count * (group.TeamGroups.Count - 1)) / 2);
        }

        public Task DeleteTeamAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _teamRepository.GetTeamByIdAsync(id);
        }

        public async Task<IEnumerable<Team>> GetTeamsAsync()
        {
            return await _teamRepository.GetTeamsAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Fetching teams for categoryId: {CategoryId}", categoryId);
            try
            {
                var teams = await _teamRepository.GetTeamsByCategoryAsync(categoryId);
                if (!teams.Any())
                {
                    _logger.LogWarning("No teams found for the given category: {CategoryId}", categoryId);
                }

                teams = teams.Where(t => t.IsPlaceholder == false || t.IsPlaceholder == null).ToList();
                return teams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams for categoryId: {CategoryId}", categoryId);
                throw;
            }
        }

        public Task UpdateTeamAsync(Team team)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Team>> GetTeamsByIdAsync(IEnumerable<int> ids)
        {
            _logger.LogInformation("Fetching teams by IDs: {Ids}", string.Join(", ", ids));

            if (!ids.Any())
            {
                _logger.LogWarning("No team IDs provided for fetching.");
                return Enumerable.Empty<Team>();
            }

            try
            {
                var teams = await _teamRepository.GetTeamsByIdAsync(ids);
                return teams ?? Enumerable.Empty<Team>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams by IDs: {Ids}", string.Join(", ", ids));
                throw;
            }
        }

        public async Task<List<Team>> GetTeamsByGroupAsync(int groupId)
        {
            _logger.LogInformation("Fetching teams for groupId: {GroupId}", groupId);
            try
            {
                var teams = await _teamRepository.GetTeamsByGroupAsync(groupId);
                if (!teams.Any())
                {
                    _logger.LogWarning("No teams found for the given group: {GroupId}", groupId);
                }
                teams = teams.Where(t => t.IsPlaceholder == false || t.IsPlaceholder == null).ToList();
                return teams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams for groupId: {GroupId}", groupId);
                throw;
            }
        }
    }
}
