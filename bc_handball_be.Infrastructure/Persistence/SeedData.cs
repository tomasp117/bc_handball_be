using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.Actors.super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Infrastructure.Persistence
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Tournaments.Any())
            {
                var tournament = new Tournament { Name = "Polanka Cup" };
                context.Tournaments.Add(tournament);
                context.SaveChanges();

                if (!context.TournamentInstances.Any())
                {
                    var tournamentInstance = new TournamentInstance { EditionNumber = 2025, StartDate = new DateTime(2025, 6, 12), EndDate = new DateTime(2025, 6, 15), TournamentId = tournament.Id, Tournament = tournament };
                    context.TournamentInstances.Add(tournamentInstance);
                    context.SaveChanges();

                    if (!context.Categories.Any())
                    {
                        var categories = new List<Category>
                        {
                            new Category { Name = "Mini 4+1", VoitingOpen = false, TournamentInstanceId = tournamentInstance.Id },
                            new Category { Name = "Mini 6+1", VoitingOpen = false, TournamentInstanceId = tournamentInstance.Id },
                            new Category { Name = "Mladší žáci", VoitingOpen = false, TournamentInstanceId = tournamentInstance.Id },
                            new Category { Name = "Starší žáci", VoitingOpen = false, TournamentInstanceId = tournamentInstance.Id },
                            new Category { Name = "Mladší dorostenci", VoitingOpen = false, TournamentInstanceId = tournamentInstance.Id }
                        };

                        context.Categories.AddRange(categories);
                        context.SaveChanges();
                    }

                    if (!context.Clubs.Any())
                    {
                        var clubs = new List<Club>
                        {
                            new Club { Name = "SKH Polanka nad Odrou", Logo = "https://placehold.co/150x150", Email = "polanka@email.cz" },
                            new Club { Name = "TJ Sokol Hranice", Logo = "https://placehold.co/150x150", Email = "hranice@email.cz" },
                            new Club { Name = "HC Zubří", Logo = "https://placehold.co/150x150", Email = "zubri@email.cz" },
                            new Club { Name = "TJ Nové Veselí", Logo = "https://placehold.co/150x150", Email = "noveveseli@email.cz" },
                            new Club { Name = "HC Dukla Praha", Logo = "https://placehold.co/150x150", Email = "dukla@email.cz" },
                            new Club { Name = "TJ Cement Hranice", Logo = "https://placehold.co/150x150", Email = "cementhranice@email.cz" },
                            new Club { Name = "HC Karviná", Logo = "https://placehold.co/150x150", Email = "karvina@email.cz" },
                            new Club { Name = "HCB Brno", Logo = "https://placehold.co/150x150", Email = "hcbbrno@email.cz" }
                        };
                        context.Clubs.AddRange(clubs);
                        context.SaveChanges();
                    }

                    //if (!context.Groups.Any())
                    //{
                    //    var groups = new List<Group>
                    //    {
                    //        new Group { Name = "Skupina A", CategoryId = context.Categories.First().Id },
                    //        new Group { Name = "Skupina B", CategoryId = context.Categories.First().Id },
                    //        new Group { Name = "Skupina C", CategoryId = context.Categories.First().Id },
                    //        new Group { Name = "Skupina D", CategoryId = context.Categories.First().Id },
                    //        new Group { Name = "Skupina A", CategoryId = context.Categories.Skip(1).First().Id },
                    //        new Group { Name = "Skupina B", CategoryId = context.Categories.Skip(1).First().Id },
                    //        new Group { Name = "Skupina C", CategoryId = context.Categories.Skip(1).First().Id },
                    //        new Group { Name = "Skupina D", CategoryId = context.Categories.Skip(1).First().Id },
                    //        new Group { Name = "Skupina A", CategoryId = context.Categories.Skip(2).First().Id },
                    //        new Group { Name = "Skupina B", CategoryId = context.Categories.Skip(2).First().Id },
                    //        new Group { Name = "Skupina C", CategoryId = context.Categories.Skip(2).First().Id },
                    //        new Group { Name = "Skupina D", CategoryId = context.Categories.Skip(2).First().Id },
                    //        new Group { Name = "Skupina A", CategoryId = context.Categories.Skip(3).First().Id },
                    //        new Group { Name = "Skupina B", CategoryId = context.Categories.Skip(3).First().Id },
                    //        new Group { Name = "Skupina C", CategoryId = context.Categories.Skip(3).First().Id },
                    //        new Group { Name = "Skupina D", CategoryId = context.Categories.Skip(3).First().Id },
                    //        new Group { Name = "Skupina A", CategoryId = context.Categories.Skip(4).First().Id },
                    //        new Group { Name = "Skupina B", CategoryId = context.Categories.Skip(4).First().Id },
                    //        new Group { Name = "Skupina C", CategoryId = context.Categories.Skip(4).First().Id },
                    //        new Group { Name = "Skupina D", CategoryId = context.Categories.Skip(4).First().Id }
                    //    };
                    //    context.Groups.AddRange(groups);
                    //    context.SaveChanges();
                    //}

                    if (!context.Teams.Any())
                    {
                        var teams = new List<Team>();
                        var categories = context.Categories.ToList();
                        var clubs = context.Clubs.ToList();
                        //var groups = context.Groups.ToList();
                        tournamentInstance = context.TournamentInstances.First();

                        int clubIdx = 0;
                        int j = 0;
                        foreach (var category in categories)
                        {
                            
                            //var categoryGroups = groups.Where(g => g.CategoryId == category.Id).ToList();

                            //foreach (var group in categoryGroups)
                            //{
                                for (int i = 1; i <= 18 + j; i++)
                                {
                                    var team = new Team
                                    {
                                        Name = $"{clubs[clubIdx % clubs.Count].Name} {category.Name}{i}",
                                        ClubId = clubs[clubIdx % clubs.Count].Id,
                                        CategoryId = category.Id,
                                        TournamentInstanceId = tournamentInstance.Id,
                                        GroupId = null,
                                    };
                                    teams.Add(team);
                                    clubIdx++;
                                }

                            j++;
                            //}
                        }

                        context.Teams.AddRange(teams);
                        context.SaveChanges();

                    }

                    //if (!context.Players.Any())
                    //{
                    //    var players = new List<Player>();
                    //    var teams = context.Teams.ToList();
                    //    var rnd = new Random();

                    //    foreach (var team in teams)
                    //    {
                    //        for (int i = 1; i <= rnd.Next(10, 14); i++) // Každý tým má 10-14 hráčů
                    //        {
                    //            players.Add(new Player
                    //            {
                    //                FirstName = $"Player{i}",
                    //                LastName = $"Team{team.Id}",
                    //                Number = i,
                    //                GoalCount = 0,
                    //                SevenMeterGoalCount = 0,
                    //                SevenMeterMissCount = 0,
                    //                TwoMinPenaltyCount = 0,
                    //                RedCardCount = 0,
                    //                YellowCardCount = 0,
                    //                TeamId = team.Id,
                    //                CategoryId = team.CategoryId
                    //            });
                    //        }
                    //    }

                    //    context.Players.AddRange(players);
                    //    context.SaveChanges();
                    //}

                    //if (!context.Matches.Any())
                    //{
                    //    var matches = new List<Match>();
                    //    var groups = context.Groups.ToList();
                    //    var rnd = new Random();

                    //    foreach (var group in groups)
                    //    {
                    //        var teams = context.Teams.Where(t => t.GroupId == group.Id).ToList();
                    //        for (int i = 0; i < teams.Count; i++)
                    //        {
                    //            for (int j = i + 1; j < teams.Count; j++)
                    //            {
                    //                matches.Add(new Match
                    //                {
                    //                    Time = DateTime.Now.AddDays(rnd.Next(1, 3)), 
                    //                    TimePlayed = "00:00",
                    //                    Playground = $"Hřiště {rnd.Next(1, 5)}", 
                    //                    Score = "0:0",
                    //                    State = MatchState.None,
                    //                    HomeTeamId = teams[i].Id,
                    //                    AwayTeamId = teams[j].Id,
                    //                    GroupId = group.Id
                    //                });
                    //            }
                    //        }
                    //    }

                    //    context.Matches.AddRange(matches);
                    //    context.SaveChanges();
                    //}

                    if (!context.Persons.Any())
                    {
                        var persons = new List<Person>();
                        var logins = new List<Login>();
                        var rnd = new Random();

                        // Přidání trenérů
                        var teams = context.Teams.ToList();
                        foreach (var team in teams)
                        {
                            var person = new Person
                            {
                                FirstName = "Coach",
                                LastName = $"Team{team.Id}",
                                Email = $"coach{team.Id}@email.com",
                                PhoneNumber = $"12345678{team.Id}",
                                Address = "Coach Address",
                                DateOfBirth = new DateTime(1985, 1, 1)
                            };

                            var login = new Login
                            {
                                Username = $"coach{team.Id}",
                                Person = person
                            };
                            login.SetPassword("password");

                            var coach = new Coach
                            {
                                Person = person,
                                TeamId = team.Id,
                                CategoryId = team.CategoryId,
                                License = (char)('A' + rnd.Next(0, 3))
                            };

                            persons.Add(person);
                            logins.Add(login);
                            context.Coaches.Add(coach);
                        }

                        // Přidání rozhodčích
                        for (int i = 1; i <= 10; i++)
                        {
                            var person = new Person
                            {
                                FirstName = $"Referee",
                                LastName = $"Number{i}",
                                Email = $"referee{i}@email.com",
                                PhoneNumber = $"98765432{i}",
                                Address = "Referee Address",
                                DateOfBirth = new DateTime(1980, 5, 10)
                            };

                            var login = new Login
                            {
                                Username = $"referee{i}",
                                Person = person
                            };
                            login.SetPassword("password");

                            var referee = new Referee
                            {
                                Person = person,
                                License = (char)('A' + rnd.Next(0, 3))
                            };

                            persons.Add(person);
                            logins.Add(login);
                            context.Referees.Add(referee);
                        }

                        // Přidání adminů
                        for (int i = 1; i <= 2; i++)
                        {
                            var person = new Person
                            {
                                FirstName = $"Admin",
                                LastName = $"Number{i}",
                                Email = $"admin{i}@email.com",
                                PhoneNumber = $"99999999{i}",
                                Address = "Admin Address",
                                DateOfBirth = new DateTime(1975, 3, 15)
                            };

                            var login = new Login
                            {
                                Username = $"admin{i}",
                                Person = person
                            };
                            login.SetPassword("password");

                            var admin = new Admin
                            {
                                Person = person
                            };

                            persons.Add(person);
                            logins.Add(login);
                            context.Admins.Add(admin);
                        }

                        // Přidání zapisovatelů
                        for (int i = 1; i <= 5; i++)
                        {
                            var person = new Person
                            {
                                FirstName = $"Recorder",
                                LastName = $"Number{i}",
                                Email = $"recorder{i}@email.com",
                                PhoneNumber = $"55555555{i}",
                                Address = "Recorder Address",
                                DateOfBirth = new DateTime(1990, 7, 20)
                            };

                            var login = new Login
                            {
                                Username = $"recorder{i}",
                                Person = person
                            };
                            login.SetPassword("password");

                            var recorder = new Recorder
                            {
                                Person = person
                            };

                            persons.Add(person);
                            logins.Add(login);
                            context.Recorders.Add(recorder);
                        }

                        // Přidání hráčů
                        foreach (var team in teams)
                        {
                            for (int i = 1; i <= rnd.Next(10, 14); i++) // Každý tým má 10-14 hráčů
                            {
                                var person = new Person
                                {
                                    FirstName = $"Player{i}",
                                    LastName = $"Team{team.Id}",
                                    Email = $"player{i}{team.Id}@email.com",
                                    PhoneNumber = $"77777777{i}",
                                    Address = "Player Address",
                                    DateOfBirth = new DateTime(2005, 6, rnd.Next(1, 30))
                                };

                                var login = new Login
                                {
                                    Username = $"player{i}{team.Id}",
                                    Person = person
                                };
                                login.SetPassword("password");

                                var player = new Player
                                {
                                    Person = person,
                                    Number = i,
                                    GoalCount = 0,
                                    SevenMeterGoalCount = 0,
                                    SevenMeterMissCount = 0,
                                    TwoMinPenaltyCount = 0,
                                    RedCardCount = 0,
                                    YellowCardCount = 0,
                                    TeamId = team.Id,
                                    CategoryId = team.CategoryId
                                };

                                persons.Add(person);
                                logins.Add(login);
                                context.Players.Add(player);
                            }
                        }

                        // Přidání ClubAdminů
                        var clubs = context.Clubs.ToList();
                        foreach (var club in clubs)
                        {
                            var person = new Person
                            {
                                FirstName = $"ClubAdmin",
                                LastName = $"{club.Name}",
                                Email = $"admin{club.Id}@email.com",
                                PhoneNumber = $"66666666{club.Id}",
                                Address = "Club Admin Address",
                                DateOfBirth = new DateTime(1980, 4, 15)
                            };

                            var login = new Login
                            {
                                Username = $"clubadmin{club.Id}",
                                Person = person
                            };
                            login.SetPassword("password");

                            var clubAdmin = new ClubAdmin
                            {
                                Person = person,
                                ClubId = club.Id
                            };

                            persons.Add(person);
                            logins.Add(login);
                            context.ClubAdmins.Add(clubAdmin);
                        }

                        // Uložení osob a loginů do databáze
                        context.Persons.AddRange(persons);
                        context.Logins.AddRange(logins);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
