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

            var areExisting = await _groupRepository.GetGroupsByCategoryAsync(categoryId);
            if (areExisting.Any())
            {
                await _groupRepository.DeleteGroupsAsync(categoryId);
                _logger.LogWarning("Groups already exist for category {CategoryId}. This will overwrite existing groups.", categoryId);
            }

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
            var matches = await _matchRepository.GetMatchesByGroupIdAsync(groupId);
            var teamsInGroup = await _teamRepository.GetTeamsByGroupAsync(groupId);

            // 1) Globální statistiky
            var standings = teamsInGroup
                .Select(t => new GroupStanding
                {
                    TeamId = t.Id,
                    TeamName = t.Name,
                    MatchesPlayed = 0,
                    GoalsFor = 0,
                    GoalsAgainst = 0,
                    Wins = 0,
                    Draws = 0,
                    Losses = 0,
                    Points = 0
                })
                .ToDictionary(s => s.TeamId);

            foreach (var m in matches)
            {
                if (m.State != MatchState.Done ||
                    !m.HomeScore.HasValue || !m.AwayScore.HasValue ||
                    !m.HomeTeamId.HasValue || !m.AwayTeamId.HasValue)
                    continue;

                var home = standings[m.HomeTeamId.Value];
                var away = standings[m.AwayTeamId.Value];

                home.MatchesPlayed++; away.MatchesPlayed++;
                home.GoalsFor += m.HomeScore.Value;
                home.GoalsAgainst += m.AwayScore.Value;
                away.GoalsFor += m.AwayScore.Value;
                away.GoalsAgainst += m.HomeScore.Value;

                if (m.HomeScore > m.AwayScore)
                {
                    home.Wins++; home.Points += 2;
                    away.Losses++;
                }
                else if (m.HomeScore < m.AwayScore)
                {
                    away.Wins++; away.Points += 2;
                    home.Losses++;
                }
                else
                {
                    home.Draws++; home.Points += 1;
                    away.Draws++; away.Points += 1;
                }
            }

            // 2) Vezmeme všechny týmy a předáme do rekurzivní funkce
            var all = standings.Values.ToList();
            return SortGroup(all, matches);
        }

        // Rekurzivně setřídí seznam týmů podle celkových bodů a pak mini-ligy
        private List<GroupStanding> SortGroup(
            List<GroupStanding> group,
            List<Match> allMatches)
        {
            // rozdělíme podle celkových bodů
            var buckets = group
                .GroupBy(t => t.Points)
                .OrderByDescending(g => g.Key);

            var result = new List<GroupStanding>();
            foreach (var bucket in buckets)
            {
                var tied = bucket.ToList();
                if (tied.Count == 1)
                {
                    // jedinec → rovnou přidáme
                    result.Add(tied[0]);
                }
                else
                {
                    // 1) filtrujeme jejich vzájemné zápasy
                    var miniMatches = allMatches
                        .Where(m => m.State == MatchState.Done
                                    && tied.Any(t => t.TeamId == m.HomeTeamId)
                                    && tied.Any(t => t.TeamId == m.AwayTeamId))
                        .ToList();

                    // 2) spočítáme mini-statistiky
                    var miniStats = tied
                        .Select(team => {
                            int miniPts = 0, miniGd = 0, miniFor = 0;
                            foreach (var m in miniMatches)
                            {
                                bool isHome = m.HomeTeamId == team.TeamId;
                                bool isAway = m.AwayTeamId == team.TeamId;
                                if (!isHome && !isAway) continue;

                                int scTeam = isHome ? m.HomeScore.Value : m.AwayScore.Value;
                                int scOpp = isHome ? m.AwayScore.Value : m.HomeScore.Value;

                                // mini-body
                                if (scTeam > scOpp) miniPts += 2;
                                else if (scTeam == scOpp) miniPts += 1;
                                // mini-rozdíl
                                miniGd += (scTeam - scOpp);
                                // mini-vstřelené góly
                                miniFor += scTeam;
                            }
                            return new
                            {
                                Team = team,
                                MiniPts = miniPts,
                                MiniGd = miniGd,
                                MiniFor = miniFor
                            };
                        })
                        .ToList();

                    // 3) seřadíme podle mini-kritérií
                    miniStats.Sort((x, y) =>
                    {
                        int c;
                        // a) mini-body
                        c = y.MiniPts.CompareTo(x.MiniPts);
                        if (c != 0) return c;
                        // b) mini-rozdíl gólů
                        c = y.MiniGd.CompareTo(x.MiniGd);
                        if (c != 0) return c;
                        // c) mini-vstřelené góly
                        c = y.MiniFor.CompareTo(x.MiniFor);
                        if (c != 0) return c;
                        // d) globální rozdíl gólů
                        int diffX = x.Team.GoalsFor - x.Team.GoalsAgainst;
                        int diffY = y.Team.GoalsFor - y.Team.GoalsAgainst;
                        c = diffY.CompareTo(diffX);
                        if (c != 0) return c;
                        // e) globální vstřelené góly
                        c = y.Team.GoalsFor.CompareTo(x.Team.GoalsFor);
                        if (c != 0) return c;
                        // f) fallback abecedně
                        return string.Compare(
                          x.Team.TeamName,
                          y.Team.TeamName,
                          StringComparison.Ordinal);
                    });

                    // 4) vezmeme z miniStats nejprve tu první trojici (či kolik tam je shod)
                    var sortedTeams = miniStats.Select(ms => ms.Team).ToList();
                    // 5) jestli zůstali nějací se stejnými mini-body atd., rekurzivně na ně
                    //    (volíme tu první skupinu, která je současně v 'tied' a mají úplně
                    //     stejná X,Y,Z),
                    //    ale protože jsme kompletně seřadili celou tied, prostě ho převedeme:
                    result.AddRange(sortedTeams);
                }
            }
            return result;
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
