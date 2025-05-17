using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using bc_handball_be.Core.Services.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class MatchService : IMatchService
    {
        private readonly ILogger<MatchService> _logger;
        private readonly IMatchRepository _matchRepository;
        private readonly ICategoryService _categoryService;
        private readonly ITeamService _teamService;
        private readonly IGroupService _groupService;
        private readonly IClubService _clubService;
        private readonly Dictionary<int, List<CourtRule>> _categoryCourtRules = new();


        public MatchService(ILogger<MatchService> logger, IMatchRepository matchRepository, ICategoryService categoryService, ITeamService teamService, IGroupService groupService, IClubService clubService)
        {
            _logger = logger;
            _matchRepository = matchRepository;
            _categoryService = categoryService;
            _teamService = teamService;
            _groupService = groupService;
            _clubService = clubService;
        }

        private int NumberOfMatchesInGroup(int teamsPerGroup)
        {
            return (teamsPerGroup * (teamsPerGroup - 1)) / 2;
        }

        private int NumberOfMatchesCommon3(int teamsPerGroup, int groupCount)
        {
            int final = 2;
            int semiFinal = 2;
            int forSemiFinal = 3;

            int knownMatches = final + semiFinal + forSemiFinal;

            return groupCount * NumberOfMatchesInGroup(teamsPerGroup) + knownMatches + (teamsPerGroup - 2) * ((groupCount * (groupCount - 1)) / 2);
        }

        private int NumberOfMatchesCommon4(int teamsPerGroup, int groupCount)
        {
            int final = 2;
            int semiFinal = 2;
            int quaterFinal = 4;

            int knownMatches = final + semiFinal + quaterFinal;

            return groupCount * NumberOfMatchesInGroup(teamsPerGroup) + knownMatches + (teamsPerGroup - 1);
        }

        private int NumberOfMatchesCommon5(int teamsPerGroup, int groupCount)
        {
            int final = 2;
            int semiFinal = 2;
            int quaterFinal = 4;
            int forQuaterFinal = 4;
            int forQuaterFinalLooser = 1;
            int forFifthPlace = 4;
            int forNinePlace = 1;

            int knownMatches = final + semiFinal + quaterFinal + forQuaterFinal + forQuaterFinalLooser + forFifthPlace + forNinePlace;

            return groupCount * NumberOfMatchesInGroup(teamsPerGroup) + knownMatches;
        }

        private async Task<int> GetNumOfMatches(int category)
        {
            var teams = await _teamService.GetTeamsByCategoryAsync(category);
            var teamCount = teams.Count();
            var groups = await _groupService.GetGroupsByCategoryAsync(category);
            var groupCount = groups.Count();
            var teamsPerGroup = 0;

            int numberOfMatches = 0;

            switch (groupCount)
            {
                case 3:
                    _logger.LogInformation("Generating matches for category {category} with 3 groups", category);
                    numberOfMatches = 0;

                    if (teamCount % groupCount == 0)
                    {
                        teamsPerGroup = teamCount / groupCount;
                        numberOfMatches = NumberOfMatchesCommon3(teamsPerGroup, groupCount);
                    }
                    else if (teamCount % groupCount == 1)
                    {
                        teamsPerGroup = (teamCount - 1) / groupCount;
                        numberOfMatches = NumberOfMatchesCommon3(teamsPerGroup, groupCount) + groupCount + teamsPerGroup;
                    }
                    else if (teamCount % groupCount == 2)
                    {
                        teamsPerGroup = (teamCount - 2) / groupCount;
                        numberOfMatches = NumberOfMatchesCommon3(teamsPerGroup, groupCount) + (teamsPerGroup * 2) + 1;
                    }
                    break;

                case 4:
                    _logger.LogInformation("Generating matches for category {category} with 4 groups", category);
                    numberOfMatches = 0;

                    if (teamCount % groupCount == 0)
                    {
                        teamsPerGroup = teamCount / groupCount;
                        numberOfMatches = NumberOfMatchesCommon4(teamsPerGroup, groupCount) * groupCount;
                    }
                    else if (teamCount % groupCount == 1)
                    {
                        teamsPerGroup = (teamCount - 1) / groupCount;
                        numberOfMatches = NumberOfMatchesCommon4(teamsPerGroup, groupCount) * groupCount + 2 + teamsPerGroup;
                    }
                    else if (teamCount % groupCount == 2 || teamCount % groupCount == 3)
                    {
                        int mod = teamCount % groupCount;
                        teamsPerGroup = (teamCount - mod) / groupCount;
                        numberOfMatches = NumberOfMatchesCommon4(teamsPerGroup, groupCount) * groupCount + ((mod * (mod - 1)) / 2);
                    }
                    break;
                case 5:
                    _logger.LogInformation("Generating matches for category {category} with 5 groups", category);
                    numberOfMatches = 0;

                    if (teamCount % groupCount == 0)
                    {
                        teamsPerGroup = teamCount / groupCount;
                        numberOfMatches = NumberOfMatchesCommon5(teamsPerGroup, groupCount) + (5 * (teamsPerGroup - 2));
                    }
                    else if (teamCount % groupCount == 1)
                    {
                        teamsPerGroup = (teamCount - 1) / groupCount;
                        numberOfMatches = NumberOfMatchesCommon5(teamsPerGroup, groupCount) + (5 * (teamsPerGroup - 3)) + 9 + teamsPerGroup;
                    }
                    else if (teamCount % groupCount == 2 || teamCount % groupCount == 3 || teamCount % groupCount == 4)
                    {
                        int mod = teamCount % groupCount;
                        teamsPerGroup = (teamCount - mod) / groupCount;
                        numberOfMatches = NumberOfMatchesCommon5(teamsPerGroup, groupCount) + (5 * (teamsPerGroup - 3)) + 3 + (teamsPerGroup * mod);
                    }
                    break;
            }

            return numberOfMatches;
        }

        /*public async List<Match> GeneratePlayoffMatches3Groups()
        {
            var categories = await _categoryService.GetCategoriesAsync();

            var newClub = new Club
            {
                Name = "Pseudo3 Club",
                Logo = "https://example.com/logo.png",
                Email = "Pseudo3@club.com",
                Address = "123 Pseudo St, Pseudo City",
            };

            await _clubService.AddClubAsync(newClub);

            foreach (var category in categories)
            {
                var groups = await _groupService.GetGroupsByCategoryAsync(category.Id);
                var pseudoTeams = new List<Team>();

                
                foreach (var group in groups)
                {
                    var teams = await _teamService.GetTeamsByGroupAsync(group.Id);
                    var groupLetter = group.Name.Last();

                    for (int i = 0; i < teams.Count; i++)
                    {
                        var pseudoTeam = new Team
                        {
                            Name = $"{i + 1}.{groupLetter}",
                            CategoryId = category.Id,
                            TournamentInstanceId = category.TournamentInstanceId,
                            ClubId = newClub.Id
                        };
                        await _teamService.AddTeamAsync(pseudoTeam); 
                        pseudoTeams.Add(pseudoTeam);
                    }
                }

                int teamsPerGroup = pseudoTeams.Count() / groups.Count();
                int mod = pseudoTeams.Count() % groups.Count();

                var newGroups = new List<Group>();

                int final = 2;
                int semiFinal = 2;
                int forSemiFinal = 3;

                int knownMatches = final + semiFinal + forSemiFinal;

                int result = knownMatches + (teamsPerGroup - 2) * ((groups.Count() * (groups.Count() - 1)) / 2);

                if (mod == 0)
                {
                    
                }
                else if (mod == 1)
                {
                    
                }
                else if (mod == 2)
                {
                    
                }



            }
        }*/

        public async Task<List<Match>> GenBlankMatches(int edition)
        {
            var matches = new List<Match>();
            var slots = GenerateMatchSlots().OrderBy(x => x.Time).ToList();

            var categories = await _categoryService.GetCategoriesAsync(edition);

            int sumOfMatches = 0;
            foreach (var category in categories)
            {
                sumOfMatches += await GetNumOfMatches(category.Id);
            }

            for (int i = 0; i < sumOfMatches && i < slots.Count; i++)
            {
                var slot = slots[i];
                matches.Add(new Match
                {
                    Time = slot.Time,
                    TimePlayed = "00:00",
                    Playground = slot.Court,
                    HomeScore = null,
                    AwayScore = null,
                    State = MatchState.Generated
                });
            }

            await _matchRepository.AddMatchesAsync(matches);

            return matches;
        }

        private List<(DateTime Time, string Court)> GenerateMatchSlots()
        {
            var result = new List<(DateTime Time, string Court)>();

            var tournamentStart = new DateTime(2025, 6, 13);

            foreach (var schedule in schedules)
            {
                int dayOffset = ((int)schedule.Day - (int)DayOfWeek.Friday + 7) % 7;
                var date = tournamentStart.AddDays((int)dayOffset);

                var current = date.Date + schedule.Start;
                var end = date.Date + schedule.End;

                while (current + TimeSpan.FromMinutes(schedule.IntervalMinutes) <= end)
                {
                    result.Add((current, schedule.CourtName));
                    current = current.AddMinutes(schedule.IntervalMinutes);
                }
            }

            return result;
        }

        private List<(int teamHomeId, int teamAwayId)> GetTeamPairsInGroup(List<Team> teams)
        {
            var matches = new List<(int, int)>();
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    matches.Add((teams[i].Id, teams[j].Id));
                }
            }
            return matches;
        }


        private bool CourtAllowedForCategory(int categoryId, string court, DayOfWeek day)
        {
            if (!_categoryCourtRules.TryGetValue(categoryId, out var rules))
            {
                _logger.LogWarning("Kategorie s ID {id} nemá definovaná žádná pravidla hřišť.", categoryId);
                return false;
            }

            foreach (var rule in rules)
            {
                if (rule.Court == court && (rule.AllowedDays == null || rule.AllowedDays.Contains(day)))
                {
                    return true;
                }
            }

            return false;
        }


        private bool HasEnoughRest(Dictionary<int, DateTime> lastMatchTimeForTeam, int teamId, DateTime proposedTime)
        {
            if (!lastMatchTimeForTeam.TryGetValue(teamId, out var lastTime))
                return true;

            return (proposedTime - lastTime).TotalMinutes >= 70;
        }

        private bool IsLunchTime(DateTime time)
        {
            var start = time.Date + new TimeSpan(11, 0, 0);
            var end = time.Date + new TimeSpan(12, 30, 0);
            return time >= start && time < end;
        }



        private bool HadEnoughLunchBreak(int teamId, DateTime proposedTime, List<Match> allAssignedMatches)
        {
            var lunchMatch = allAssignedMatches
                .Where(m => IsLunchTime(m.Time) && (m.HomeTeamId == teamId || m.AwayTeamId == teamId))
                .OrderByDescending(m => m.Time)
                .FirstOrDefault();

            if (lunchMatch == null)
                return true;

            int matchesAfter = allAssignedMatches
                .Count(m => m.Time > lunchMatch.Time && m.Time < proposedTime);

            return matchesAfter >= 3;
        }



        private class CourtSchedule
        {
            public string CourtName { get; set; } = "";
            public DayOfWeek Day { get; set; }
            public TimeSpan Start { get; set; }
            public TimeSpan End { get; set; }
            public int IntervalMinutes { get; set; } = 35;
        }

        private List<CourtSchedule> schedules = new List<CourtSchedule>
        {
            new CourtSchedule() { CourtName = "Hala", Day = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
            new CourtSchedule() { CourtName = "Hala", Day = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
            new CourtSchedule() { CourtName = "Hala", Day = DayOfWeek.Sunday, Start = new TimeSpan(9, 0, 0), End = new TimeSpan(13, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 1 Tartan", Day = DayOfWeek.Friday, Start = new TimeSpan(8, 30, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 1 Tartan", Day = DayOfWeek.Saturday, Start = new TimeSpan(8, 30, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 1 Tartan", Day = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(12, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 2 Tartan", Day = DayOfWeek.Friday, Start = new TimeSpan(8, 30, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 2 Tartan", Day = DayOfWeek.Saturday, Start = new TimeSpan(8, 30, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 2 Tartan", Day = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(12, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 3 Umělá tráva", Day = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 3 Umělá tráva", Day = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 3 Umělá tráva", Day = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(12, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 4 Umělá tráva", Day = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 4 Umělá tráva", Day = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 4 Umělá tráva", Day = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(12, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 5 Tartan Dělnický dům", Day = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 5 Tartan Dělnický dům", Day = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 6 Beton Dělnický dům", Day = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(18, 30, 0) },
            new CourtSchedule() { CourtName = "Hřiště 6 Beton Dělnický dům", Day = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(18, 30, 0) },
        };



        private class CourtRule
        {
            public string Court { get; set; } = "";
            public List<DayOfWeek>? AllowedDays { get; set; } // null = každý den
            public bool IsPrimary { get; set; } = false;
        };


        private async Task InitCategoryCourtRules(int edition)
        {
            var categories = await _categoryService.GetCategoriesAsync(edition);


            var mini61 = categories.FirstOrDefault(c => c.Name == "Mini 6+1");
            var mladsiZaci = categories.FirstOrDefault(c => c.Name == "Mladší žáci");
            var starsiZaci = categories.FirstOrDefault(c => c.Name == "Starší žáci");
            var mladsiDorost = categories.FirstOrDefault(c => c.Name == "Mladší dorostenci");

            _logger.LogInformation("Kategorie: {name} => ID {id}", mladsiZaci?.Name, mladsiZaci?.Id);
            _logger.LogInformation("Kategorie: {name} => ID {id}", starsiZaci?.Name, starsiZaci?.Id);
            _logger.LogInformation("Kategorie: {name} => ID {id}", mladsiDorost?.Name, mladsiDorost?.Id);
            _logger.LogInformation("Kategorie: {name} => ID {id}", mini61?.Name, mini61?.Id);
            if (mini61 == null && mladsiZaci == null && starsiZaci == null && mladsiDorost == null)
            {
                _logger.LogWarning("Nebyly nalezeny žádné kategorie.");
                return;
            }

            if (mini61 != null)
            {
                _categoryCourtRules[mini61.Id] = new List<CourtRule>
                {
                    new CourtRule { Court = "Hřiště 3 Umělá tráva" },
                    new CourtRule { Court = "Hřiště 4 Umělá tráva" },
                    new CourtRule { Court = "Hřiště 5 Tartan Dělnický dům", AllowedDays = new() { DayOfWeek.Saturday } },
                };
            }

            if (mladsiZaci != null)
            {
                _categoryCourtRules[mladsiZaci.Id] = new List<CourtRule>
                {
                    new CourtRule { Court = "Hřiště 5 Tartan Dělnický dům", IsPrimary = true },
                    new CourtRule { Court = "Hřiště 6 Beton Dělnický dům", IsPrimary = true },
                    new CourtRule { Court = "Hřiště 1 Tartan" },
                    new CourtRule { Court = "Hřiště 2 Tartan" },
                    new CourtRule { Court = "Hřiště 3 Umělá tráva" },
                    new CourtRule { Court = "Hřiště 4 Umělá tráva" },
                };
            }

            if (starsiZaci != null)
            {
                _categoryCourtRules[starsiZaci.Id] = new List<CourtRule>
                {
                    new CourtRule { Court = "Hřiště 1 Tartan" },
                    new CourtRule { Court = "Hřiště 2 Tartan" },
                };
            }

            if (mladsiDorost != null)
            {
                _categoryCourtRules[mladsiDorost.Id] = new List<CourtRule>
                {
                    new CourtRule { Court = "Hala", IsPrimary = true},
                    new CourtRule { Court = "Hřiště 1 Tartan", AllowedDays = new() { DayOfWeek.Saturday, DayOfWeek.Sunday } },
                    new CourtRule { Court = "Hřiště 2 Tartan", AllowedDays = new() { DayOfWeek.Saturday, DayOfWeek.Sunday } },
                };
            }

            _logger.LogInformation("Pravidla hřišť pro kategorie inicializována.");
            foreach (var category in _categoryCourtRules)
            {
                _logger.LogInformation("Kategorie {id}: {rules}", category.Key, string.Join(", ", category.Value.Select(r => r.Court)));
            }
        }

        private bool IsRightAfterLunch(DateTime time)
        {
            var lunchEnd = time.Date + new TimeSpan(12, 30, 0);
            return time >= lunchEnd && time < lunchEnd.AddHours(2);
        }




        public async Task<List<Match>> AssignGroupMatchesFromScratch(int categoryId, int edition)
        {
            if (_categoryCourtRules.Count == 0)
            {
                await InitCategoryCourtRules(edition);
            }

            var teams = await _teamService.GetTeamsByCategoryAsync(categoryId);
            var groups = await _groupService.GetGroupsByCategoryAsync(categoryId);

            var categories = await _categoryService.GetCategoriesAsync(edition);

            var mini41 = categories.FirstOrDefault(c => c.Name == "Mini 4+1");

            var preferredDaysOrder = new List<DayOfWeek> { DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };


            var blankMatches = (await _matchRepository.GetMatchesByStateAsync(MatchState.Generated))
                .Where(m => m.Category?.Name != mini41?.Name)
                .Where(m => CourtAllowedForCategory(categoryId, m.Playground, m.Time.DayOfWeek))
                .OrderBy(m => preferredDaysOrder.IndexOf(m.Time.DayOfWeek)) // řadí nejdřív pátek, pak sobotu, neděli
                .ThenByDescending(m => _categoryCourtRules[categoryId].FirstOrDefault(r => r.Court == m.Playground)?.IsPrimary ?? false) // primární hřiště první
                .ThenBy(m => m.Time)
                .ToList();

            _logger.LogInformation("Počet dostupných slotů: {count}", blankMatches.Count);

            var lastMatchTimeForTeam = new Dictionary<int, DateTime>();
            var assignedMatches = new List<Match>();
            int slotIndex = 0;

            var occupiedSlots = new HashSet<int>();
            var lunchMatchTime = new Dictionary<int, DateTime?>();

            foreach (var group in groups)
            {
                var groupTeams = teams.Where(t => t.TeamGroups.Any(tg => tg.GroupId == group.Id)).ToList();

                var groupMatches = GetTeamPairsInGroup(groupTeams);

                foreach (var (homeId, awayId) in groupMatches)
                {
                    bool matchAssigned = false;
                    foreach (var slot in blankMatches.Where(s => !occupiedSlots.Contains(s.Id)))
                    {
                        // Kontroly:
                        bool canHomePlay = HasEnoughRest(lastMatchTimeForTeam, homeId, slot.Time) &&
                                           HadEnoughLunchBreak(homeId, slot.Time, assignedMatches);

                        bool canAwayPlay = HasEnoughRest(lastMatchTimeForTeam, awayId, slot.Time) &&
                                           HadEnoughLunchBreak(awayId, slot.Time, assignedMatches);

                        if (canHomePlay && canAwayPlay)
                        {
                            slot.HomeTeamId = homeId;
                            slot.AwayTeamId = awayId;
                            slot.GroupId = group.Id;
                            slot.Category = group.Category;
                            slot.State = MatchState.None;

                            await _matchRepository.UpdateMatchAsync(slot);

                            lastMatchTimeForTeam[homeId] = slot.Time;
                            lastMatchTimeForTeam[awayId] = slot.Time;

                            // Pokud hrál v čase oběda, zaznamenej:
                            if (IsLunchTime(slot.Time))
                            {
                                lunchMatchTime[homeId] = slot.Time;
                                lunchMatchTime[awayId] = slot.Time;
                            }

                            occupiedSlots.Add(slot.Id);
                            assignedMatches.Add(slot);
                            matchAssigned = true;
                            break;
                        }
                    }

                    if (!matchAssigned)
                    {
                        _logger.LogWarning("Žádný vhodný slot nenalezen pro zápas {home}-{away}.", homeId, awayId);
                    }
                }
            }


            return assignedMatches;
        }

        public async Task<List<Match>> AssignAllGroupMatchesFromScratch(int edition)
        {
            if (_categoryCourtRules.Count == 0)
                await InitCategoryCourtRules(edition);

            var categories = await _categoryService.GetCategoriesAsync(edition);
            var teams = await _teamService.GetTeamsAsync();
            var groups = await _groupService.GetGroupsAsync();
            _logger.LogInformation("Groups: {groups}", string.Join(", ", groups.Select(g => g.Name)));

            var mini41 = categories.FirstOrDefault(c => c.Name == "Mini 4+1");
            var preferredDaysOrder = new List<DayOfWeek> { DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };

            var blankMatches = (await _matchRepository.GetMatchesByStateAsync(MatchState.Generated))
                .Where(m => m.Category?.Name != mini41?.Name)
                .OrderBy(m => preferredDaysOrder.IndexOf(m.Time.DayOfWeek))
                .ThenByDescending(m =>
                {
                    if (m.Category == null || !_categoryCourtRules.TryGetValue(m.Category.Id, out var rules))
                        return false;

                    return rules.Any(r => r.Court == m.Playground && r.IsPrimary);
                })
                .ThenBy(m => m.Time)
                .ToList();

            _logger.LogInformation("Počet dostupných slotů: {count}", blankMatches.Count);

            var lastMatchTimeForTeam = new Dictionary<int, DateTime>();
            var occupiedSlots = new HashSet<int>();
            var assignedMatches = new List<Match>();

            foreach (var category in categories)
            {
                if (mini41 != null && category.Id == mini41.Id)
                    continue;

                if (!_categoryCourtRules.ContainsKey(category.Id))
                {
                    _logger.LogWarning("Kategorie {id} nemá pravidla hřišť. Přeskakuji.", category.Id);
                    continue;
                }

                var categoryTeams = teams.Where(t => t.CategoryId == category.Id).ToList();
                var categoryGroups = groups.Where(g => g.CategoryId == category.Id).ToList();

                _logger.LogInformation("Kategorie {id} ({name}) - Počet týmů: {count}", category.Id, category.Name, categoryTeams.Count);
                _logger.LogInformation("Počet skupin: {count}", categoryGroups.Count);

                foreach (var group in categoryGroups)
                {
                    var groupTeams = categoryTeams.Where(t => t.TeamGroups.Any(tg => tg.GroupId == group.Id)).ToList();
                    var groupMatches = GetTeamPairsInGroup(groupTeams);

                    _logger.LogInformation("Skupina {id} ({name}) - Počet týmů: {count}", group.Id, group.Name, groupTeams.Count);

                    foreach (var (homeId, awayId) in groupMatches)
                    {
                        bool matchAssigned = false;

                        foreach (var slot in blankMatches.Where(s =>
                                 !occupiedSlots.Contains(s.Id) &&
                                 CourtAllowedForCategory(category.Id, s.Playground, s.Time.DayOfWeek)))
                        {
                            _logger.LogInformation("Zápas {home}-{away} v kategorii {cat} na hřišti {court} v čase {time}.", homeId, awayId, category.Name, slot.Playground, slot.Time);
                            if (!CourtAllowedForCategory(category.Id, slot.Playground, slot.Time.DayOfWeek))
                            {
                                _logger.LogDebug("Slot {slotId} zamítnut kvůli pravidlům hřišť", slot.Id);
                                continue;
                            }

                            if (!HasEnoughRest(lastMatchTimeForTeam, homeId, slot.Time))
                            {
                                _logger.LogDebug("Tým {id} nemá dost odpočinku (domácí)", homeId);
                                continue;
                            }

                            if (!HasEnoughRest(lastMatchTimeForTeam, awayId, slot.Time))
                            {
                                _logger.LogDebug("Tým {id} nemá dost odpočinku (host)", awayId);
                                continue;
                            }

                            if (!HadEnoughLunchBreak(homeId, slot.Time, assignedMatches))
                            {
                                _logger.LogDebug("Tým {id} nemá dost pauzy po obědě (domácí)", homeId);
                                continue;
                            }

                            if (!HadEnoughLunchBreak(awayId, slot.Time, assignedMatches))
                            {
                                _logger.LogDebug("Tým {id} nemá dost pauzy po obědě (host)", awayId);
                                continue;
                            }

                            bool canHomePlay = HasEnoughRest(lastMatchTimeForTeam, homeId, slot.Time) &&
                                               HadEnoughLunchBreak(homeId, slot.Time, assignedMatches);

                            bool canAwayPlay = HasEnoughRest(lastMatchTimeForTeam, awayId, slot.Time) &&
                                               HadEnoughLunchBreak(awayId, slot.Time, assignedMatches);

                            if (canHomePlay && canAwayPlay)
                            {
                                slot.HomeTeamId = homeId;
                                slot.AwayTeamId = awayId;
                                slot.GroupId = group.Id;
                                slot.Category = group.Category;
                                slot.State = MatchState.None;

                                await _matchRepository.UpdateMatchAsync(slot);

                                _logger.LogInformation("Přiřazen zápas {home}-{away} v kategorii {cat} na hřišti {court} v čase {time}.", homeId, awayId, category.Name, slot.Playground, slot.Time);

                                lastMatchTimeForTeam[homeId] = slot.Time;
                                lastMatchTimeForTeam[awayId] = slot.Time;

                                occupiedSlots.Add(slot.Id);
                                assignedMatches.Add(slot);
                                matchAssigned = true;


                                break;
                            }
                        }

                        if (!matchAssigned)
                        {
                            //_logger.LogWarning("Žádný vhodný slot nenalezen pro zápas {home}-{away} v kategorii {cat}.", homeId, awayId, category.Name);
                        }
                    }
                }
            }

            return assignedMatches;
        }



        public async Task<List<Match>> GetMatchesAsync()
        {
            var matches = await _matchRepository.GetMatchesAsync();
            return matches;
        }

        public async Task<List<UnassignedMatch>> GetUnassignedGroupMatches(int categoryId)
        {
            var groups = await _groupService.GetGroupsByCategoryAsync(categoryId);
            var teams = await _teamService.GetTeamsByCategoryAsync(categoryId);

            var allMatches = await _matchRepository.GetMatchesAsync();
            var realMatches = allMatches.Where(m => m.HomeTeamId.HasValue && m.AwayTeamId.HasValue && m.State != MatchState.Generated).ToList();

            var result = new List<UnassignedMatch>();

            foreach (var group in groups)
            {
                var groupTeams = teams.Where(t => t.TeamGroups.Any(tg => tg.GroupId == group.Id)).ToList();

                for (int i = 0; i < groupTeams.Count; i++)
                {
                    for (int j = i + 1; j < groupTeams.Count; j++)
                    {
                        bool alreadyAssigned = realMatches.Any(m =>
                            (m.HomeTeamId == groupTeams[i].Id && m.AwayTeamId == groupTeams[j].Id) ||
                            (m.HomeTeamId == groupTeams[j].Id && m.AwayTeamId == groupTeams[i].Id)
);

                        if (!alreadyAssigned)
                        {
                            result.Add(new UnassignedMatch
                            {
                                HomeTeam = groupTeams[i],
                                AwayTeam = groupTeams[j],
                                Group = group,
                            });
                        }
                    }
                }
            }

            return result;
        }

        public async Task UpdateMatchesAsync(List<Match> assignments)
        {
            foreach (var assigned in assignments)
            {
                var match = await _matchRepository.GetMatchByIdAsync(assigned.Id);
                if (match == null)
                {
                    _logger.LogWarning("Zápas s ID {Id} nebyl nalezen.", assigned.Id);
                    continue;
                }

                match.HomeTeamId = assigned.HomeTeamId;
                match.AwayTeamId = assigned.AwayTeamId;
                match.GroupId = assigned.GroupId;
                match.State = MatchState.None;
            }

            await _matchRepository.SaveAsync();
        }

        public async Task<Match> GetMatchByIdAsync(int id)
        {
            var match = await _matchRepository.GetMatchByIdAsync(id);
            if (match == null)
            {
                _logger.LogWarning("Zápas s ID {Id} nebyl nalezen.", id);
                return null;
            }
            return match;
        }

        public async Task UpdateMatchAsync(Match match)
        {
            await _matchRepository.UpdateMatchAsync(match);
        }

        public async Task<List<Match>> GetMatchesByCategoryIdAsync(int categoryId)
        {
            var matches = await _matchRepository.GetMatchesByCategoryIdAsync(categoryId);
            return matches;
        }

        public async Task<List<Match>> GetMatchesByGroupIdAsync(int groupId)
        {
            var matches = await _matchRepository.GetMatchesByGroupIdAsync(groupId);
            return matches;
        }

        public async Task<List<Match>> GetMatchesByTeamIdAsync(int teamId)
        {
            var matches = await _matchRepository.GetMatchesByTeamIdAsync(teamId);
            return matches;
        }
    }
    
}

