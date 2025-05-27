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
    public class GroupService : IGroupService
    {

        private readonly IGroupRepository _groupRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly ITournamentInstanceService _tournamentInstanceService;
        private readonly IClubService _clubService;
        private readonly ILogger<GroupService> _logger;

        public GroupService(IGroupRepository groupRepository, ITeamRepository teamRepository, IMatchRepository matchRepository, ILogger<GroupService> logger, ITournamentInstanceService tournamentInstanceService, IClubService clubService)
        {
            _groupRepository = groupRepository;
            _teamRepository = teamRepository;
            _matchRepository = matchRepository;
            _tournamentInstanceService = tournamentInstanceService;
            _clubService = clubService;
            _logger = logger;
        }

        public async Task SaveGroupsAsync(IEnumerable<Group> newGroups, int categoryId)
        {
            var validGroups = newGroups.Where(g => g.TeamGroups.Any()).ToList();
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
                group.TeamGroups = group.TeamGroups
                    .Where(tg => teamDictionary.ContainsKey(tg.TeamId))
                    .Select(tg => new TeamGroup
                    {
                        TeamId = tg.TeamId,
                        Group = group,
                        Team = teamDictionary[tg.TeamId]
                    })
                    .ToList();
            }

            _logger.LogInformation("Saving {Count} groups for category {CategoryId} with {TeamCount} total team-group assignments",validGroups.Count, categoryId, validGroups.Sum(g => g.TeamGroups.Count));
            await _groupRepository.SaveGroupsAsync(validGroups, categoryId);
            _logger.LogInformation("Groups saved successfully for category {CategoryId}", categoryId);
        }

        public async Task<IEnumerable<Group>> GetGroupsByCategoryAsync(int categoryId)
        {
            return await _groupRepository.GetGroupsByCategoryAsync(categoryId);
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            var groups = await _groupRepository.GetGroupsAsync();
            return groups.ToList();
        }

        public async Task<List<GroupStanding>> GetGroupStandingsAsync(int groupId)
        {
            // 1. Získáš všechny zápasy dané skupiny
            var matches = await _matchRepository.GetMatchesByGroupIdAsync(groupId);

            // 2. Získáš všechny týmy v dané skupině (i když ještě nehrály)
            var teamsInGroup = await _teamRepository.GetTeamsByGroupAsync(groupId);

            var standings = new Dictionary<int, GroupStanding>();

            // 3. Pro každý tým vytvoříš výchozí statistiku
            foreach (var team in teamsInGroup)
            {
                standings[team.Id] = new GroupStanding
                {
                    TeamId = team.Id,
                    TeamName = team.Name
                };
            }

            // 4. Projedeš zápasy a spočítáš statistiky
            foreach (var match in matches)
            {
                if(match.State != MatchState.Done)
                    continue; 
                if (match.HomeScore == null || match.AwayScore == null)
                    continue;
                if (match.HomeTeamId == null || match.AwayTeamId == null)
                    continue;

                var homeId = match.HomeTeamId.Value;
                var awayId = match.AwayTeamId.Value;

                var home = standings[homeId];
                var away = standings[awayId];

                home.MatchesPlayed++;
                away.MatchesPlayed++;

                home.GoalsFor += match.HomeScore.Value;
                home.GoalsAgainst += match.AwayScore.Value;

                away.GoalsFor += match.AwayScore.Value;
                away.GoalsAgainst += match.HomeScore.Value;

                if (match.HomeScore > match.AwayScore)
                {
                    home.Wins++;
                    home.Points += 2;
                    away.Losses++;
                }
                else if (match.HomeScore < match.AwayScore)
                {
                    away.Wins++;
                    away.Points += 2;
                    home.Losses++;
                }
                else
                {
                    home.Draws++;
                    away.Draws++;
                    home.Points += 1;
                    away.Points += 1;
                }
            }

            // 5. Vrátíš setříděné výsledky
            return standings.Values
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalsFor - s.GoalsAgainst)
                .ThenByDescending(s => s.GoalsFor)
                .ToList();
        }

        public async Task SavePlaceholderGroupsAsync(List<PlaceholderGroup> placeholderGroups, int categoryId)
        {
            if (placeholderGroups == null || !placeholderGroups.Any())
            {
                _logger.LogWarning("No placeholder groups provided for category {CategoryId}", categoryId);
                return;
            }

            _logger.LogInformation("Saving {Count} placeholder groups for category {CategoryId}", placeholderGroups.Count, categoryId);

            var tournamentInstance = await _tournamentInstanceService.GetByCategoryIdAsync(categoryId);
            var placeholderClub = await _clubService.GetPlaceholderClubAsync();

            var groups = new List<Group>();
            var usedTeamIds = new HashSet<int>();

            foreach (var input in placeholderGroups)
            {
                var group = new Group
                {
                    Name = input.Name,
                    Phase = input.Phase,
                    CategoryId = categoryId,
                    TeamGroups = new List<TeamGroup>()
                };

                foreach (var teamInput in input.Teams)
                {
                    Team team;

                    if (teamInput.Id != null)
                    {
                        // Reálný tým
                        team = await _teamRepository.GetTeamByIdAsync(teamInput.Id.Value);
                        if (team == null)
                        {
                            _logger.LogWarning("Tým s ID {TeamId} nebyl nalezen.", teamInput.Id.Value);
                            continue;
                        }

                        usedTeamIds.Add(team.Id);
                    }
                    else
                    {
                        // Placeholder tým
                        team = new Team
                        {
                            Name = teamInput.Name,
                            ClubId = placeholderClub.Id,
                            CategoryId = categoryId,
                            TournamentInstanceId = tournamentInstance.Id,
                            IsPlaceholder = true
                        };

                        await _teamRepository.AddTeamAsync(team);
                        usedTeamIds.Add(team.Id); // nový tým má ID po vložení
                    }

                    group.TeamGroups.Add(new TeamGroup
                    {
                        Team = team,
                        Group = group
                    });
                }

                groups.Add(group);
            }

            await _groupRepository.SaveGroupsAsync(groups, categoryId);

            // ⛔ Odstranit osiřelé placeholder týmy, které nejsou mezi použitémi
            var allPlaceholderTeams = await _teamRepository.GetPlaceholderTeamsByCategoryAsync(categoryId);

            var orphaned = allPlaceholderTeams
                .Where(t => !usedTeamIds.Contains(t.Id))
                .ToList();

            foreach (var team in orphaned)
            {
                await _teamRepository.DeleteTeamAsync(team.Id);
                _logger.LogInformation("Odstraněn osiřelý placeholder tým: {TeamName} (ID: {TeamId})", team.Name, team.Id);
            }
        }


        public async Task<List<Group>> GetGroupsWithPlaceholderTeamsAsync(int categoryId)
        {
            return await _groupRepository.GetGroupsWithPlaceholderTeamsAsync(categoryId);
        }
    }
}
