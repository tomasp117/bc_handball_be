﻿using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.API.DTOs.GroupBrackets;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Services.Models;
using System.Globalization;
using System.Text;

namespace bc_handball_be.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Team
            CreateMap<Team, TeamDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.TournamentInstanceNum, opt => opt.MapFrom(src => src.TournamentInstance.EditionNumber))
                .ForMember(dest => dest.GroupIds, opt => opt.MapFrom(src => src.TeamGroups.Select(tg => tg.GroupId)))
                .ForMember(dest => dest.GroupNames, opt => opt.MapFrom(src => src.TeamGroups.Select(tg => tg.Group.Name)));

            CreateMap<Team, TeamDetailDTO>()
                .IncludeBase<Team, TeamDTO>() // Základní mapování převezme z TeamDTO
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players));
            //.ForMember(dest => dest.Coaches, opt => opt.MapFrom(src => src.Coaches));

            CreateMap<TeamDTO, Team>()
                .ForMember(dest => dest.Club, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.TeamGroups, opt => opt.Ignore());

            CreateMap<Team, TeamGroupAssignDTO>();

            CreateMap<Team, BracketTeamDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<PlaceholderTeamDTO, PlaceholderTeam>();
            CreateMap<PlaceholderTeam, PlaceholderTeamDTO>();

            // Tohle stačí pro základní mapování
            CreateMap<TeamCreateDTO, Team>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.Players, opt => opt.Ignore()) 
                .ForMember(dest => dest.Coach, opt => opt.Ignore())
                .ForMember(dest => dest.Club, opt => opt.Ignore()) 
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.TournamentInstance, opt => opt.Ignore())
                .ForMember(dest => dest.TeamGroups, opt => opt.Ignore())
                .ForMember(dest => dest.HomeMatches, opt => opt.Ignore())
                .ForMember(dest => dest.AwayMatches, opt => opt.Ignore())
                .ForMember(dest => dest.IsPlaceholder, opt => opt.Ignore());


            // Player
            CreateMap<Player, PlayerDTO>()
                .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person));

            CreateMap<Player, PlayerDetailDTO>()
                .IncludeBase<Player, PlayerDTO>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null));
                //.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<PlayerDetailDTO, Player>()
                .ForMember(dest => dest.GoalCount, opt => opt.MapFrom(src => src.GoalCount))
                .ForMember(dest => dest.SevenMeterGoalCount, opt => opt.MapFrom(src => src.SevenMeterGoalCount))
                .ForMember(dest => dest.SevenMeterMissCount, opt => opt.MapFrom(src => src.SevenMeterMissCount))
                .ForMember(dest => dest.TwoMinPenaltyCount, opt => opt.MapFrom(src => src.TwoMinPenaltyCount))
                .ForMember(dest => dest.RedCardCount, opt => opt.MapFrom(src => src.RedCardCount))
                .ForMember(dest => dest.YellowCardCount, opt => opt.MapFrom(src => src.YellowCardCount))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
                .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamId))
                //.ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person));

            // Coach
            CreateMap<Coach, CoachDTO>();

            CreateMap<Coach, CoachDetailDTO>()
                .IncludeBase<Coach, CoachDTO>() // Převezme mapování z CoachDTO
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : string.Empty));
                //.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            

            CreateMap<CreateCoachDTO, Coach>()
                .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src)) // použije mapování výše
                .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamId))
                //.ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.License, opt => opt.MapFrom(src => src.License));

            // Category
            CreateMap<Category, CategoryDTO>();

            CreateMap<Category, CategoryDetailDTO>()
                .IncludeBase<Category, CategoryDTO>()
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.Groups))
                .ForMember(dest => dest.Stats, opt => opt.MapFrom(src => src.Stats));
                //.ForMember(dest => dest.Voting, opt => opt.MapFrom(src => src.Voting));

            CreateMap<CategoryPostDTO, Category>();

            CreateMap<Category, CategoryPostDTO>();

            CreateMap<CategoryDTO, Category>()
                .ForMember(dest => dest.Matches, opt => opt.Ignore())
                .ForMember(dest => dest.Groups, opt => opt.Ignore())
                //.ForMember(dest => dest.Voting, opt => opt.Ignore())
                .ForMember(dest => dest.Stats, opt => opt.Ignore())
                .ForMember(dest => dest.TournamentInstance, opt => opt.Ignore());

            // Club

            CreateMap<Club, ClubDTO>().ReverseMap();

            CreateMap<Club, ClubDetailDTO>()
                .IncludeBase<Club, ClubDTO>()
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams));

            CreateMap<ClubCsvDTO, Club>()
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src =>
                    RemoveDiacritics(src.Name.ToLower().Replace(" ", "-"))
                ));

            CreateMap<ClubDTO, Club>()
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Logo)
                        ? RemoveDiacritics(src.Name.ToLower().Replace(" ", "-"))
                        : src.Logo
                ));

            CreateMap<ClubAdmin, ClubAdminDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ClubId, opt => opt.MapFrom(src => src.ClubId))
                .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person));

            CreateMap<CreateClubAdminDTO, ClubAdmin>()
                .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src)); 
            
            CreateMap<CreateClubAdminDTO, Person>()
                .ForMember(dest => dest.Login, opt => opt.MapFrom(src => new Login { Username = src.Username }))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));


            // Event
            CreateMap<EventDTO, Event>();

            CreateMap<Event, EventDTO>();
            //.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            // Group
            CreateMap<Group, GroupDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Group, GroupDetailDTO>()
                .IncludeBase<Group, GroupDTO>()
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.TeamGroups.Select(tg => tg.Team)))
                .ForMember(dest => dest.Matches, opt => opt.MapFrom(src => src.Matches));

            CreateMap<GroupDTO, Group>();

            CreateMap<GroupDetailDTO, Group>()
                .IncludeBase<GroupDTO, Group>()
                .ForMember(dest => dest.TeamGroups, opt => opt.Ignore())
                .ForMember(dest => dest.Matches, opt => opt.MapFrom(src => src.Matches));

            CreateMap<Group, BracketGroupDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.TeamGroups.Select(tg => tg.Team)));

            CreateMap<BracketGroupDTO, Group>()
                .ForMember(dest => dest.TeamGroups, opt => opt.MapFrom(src =>
                    src.Teams.Select(t => new TeamGroup
                    {
                        TeamId = t.Id
                    }).ToList()
                ))
                .ForMember(dest => dest.Phase, opt => opt.MapFrom(src => src.Phase)) 
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<GroupStanding, GroupStandingDTO>();

            CreateMap<PlaceholderGroup, PlaceholderGroupDTO>();
            CreateMap<PlaceholderGroupDTO, PlaceholderGroup>();

            // Match
            CreateMap<Match, MatchDTO>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.ToString()))
                .ForMember(dest => dest.HomeTeam, opt => opt.MapFrom(src => src.HomeTeam))
                .ForMember(dest => dest.AwayTeam, opt => opt.MapFrom(src => src.AwayTeam))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
                .ForMember(dest => dest.Lineups, opt => opt.MapFrom(src => src.Lineups));


            CreateMap<Match, MatchDetailDTO>()
                .IncludeBase<Match, MatchDTO>()
                .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
                .ForMember(dest => dest.MainRefereeName, opt => opt.MapFrom(src => src.MainReferee != null ? $"{src.MainReferee.Person.FirstName} {src.MainReferee.Person.LastName}" : null))
                .ForMember(dest => dest.AssistantRefereeName, opt => opt.MapFrom(src => src.AssistantReferee != null ? $"{src.AssistantReferee.Person.FirstName} {src.AssistantReferee.Person.LastName}" : null));

            CreateMap<UnassignedMatch, UnassignedMatchDTO>()
                .ForMember(dest => dest.HomeTeamId, opt => opt.MapFrom(src => src.HomeTeam.Id))
                .ForMember(dest => dest.HomeTeamName, opt => opt.MapFrom(src => src.HomeTeam.Name))
                .ForMember(dest => dest.AwayTeamId, opt => opt.MapFrom(src => src.AwayTeam.Id))
                .ForMember(dest => dest.AwayTeamName, opt => opt.MapFrom(src => src.AwayTeam.Name))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Group.Id))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

            CreateMap<MatchAssignmentDTO, Match>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.MatchId))
                .ForMember(dest => dest.HomeTeamId, opt => opt.MapFrom(src => src.HomeTeamId))
                .ForMember(dest => dest.AwayTeamId, opt => opt.MapFrom(src => src.AwayTeamId))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.State, opt => opt.MapFrom(_ => MatchState.None));

            CreateMap<MatchUpdateDTO, Match>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Match, SlotDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Time, o => o.MapFrom(s => s.Time))
                .ForMember(d => d.Playground, o => o.MapFrom(s => s.Playground));
            CreateMap<SlotDTO, Match>();

            // Lineup
            CreateMap<Lineup, LineupDTO>();
            CreateMap<LineupPlayer, LineupPlayerDTO>();

            // Tournament
            CreateMap<Tournament, TournamentDTO>();
            CreateMap<TournamentDTO, Tournament>();

            // TournamentInstance
            CreateMap<TournamentInstance, TournamentInstanceDTO>()
                .ForMember(dest => dest.TournamentName, opt => opt.MapFrom(src => src.Tournament.Name))
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));

            CreateMap<TournamentInstanceDTO, TournamentInstance>()
                .ForMember(dest => dest.Tournament, opt => opt.Ignore())
                .ForMember(dest => dest.Teams, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore());


            // Person
            CreateMap<Person, PersonDTO>();

            CreateMap<PersonDTO, Person>();

            CreateMap<RegisterDTO, Person>();


            CreateMap<Person, PersonDetailDTO>()
                .IncludeBase<Person, PersonDTO>();

            CreateMap<CreateCoachDTO, Person>()
                .ForMember(dest => dest.Login, opt => opt.MapFrom(src => new Login
                {
                    Username = src.Username,
                    Password = "", 
                    Salt = ""
                }));

            //// Referee
            //CreateMap<Referee, RefereeDTO>()
            //    .IncludeBase<Person, PersonDTO>();

            //CreateMap<Referee, RefereeDetailDTO>()
            //    .IncludeBase<Referee, RefereeDTO>()
            //    .ForMember(dest => dest.MainRefereeMatchIds, opt => opt.MapFrom(src => src.MainRefereeMatches.Select(m => m.Id)))
            //    .ForMember(dest => dest.AssistantRefereeMatchIds, opt => opt.MapFrom(src => src.AssistantRefereeMatches.Select(m => m.Id)));


        }

        private static string RemoveDiacritics(string input)
        {
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
