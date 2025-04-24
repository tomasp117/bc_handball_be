using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Services.Models;

namespace bc_handball_be.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Team
            CreateMap<Team, TeamDTO>()
            .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Club.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.TournamentInstanceNum, opt => opt.MapFrom(src => src.TournamentInstance.EditionNumber))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group != null ? src.Group.Name : null));

            CreateMap<Team, TeamDetailDTO>()
            .IncludeBase<Team, TeamDTO>() // Základní mapování převezme z TeamDTO
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players))
            .ForMember(dest => dest.Coaches, opt => opt.MapFrom(src => src.Coaches));

            CreateMap<TeamDTO, Team>()
                .ForMember(dest => dest.Club, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore()) 
                .ForMember(dest => dest.Group, opt => opt.Ignore()); 


            CreateMap<Team, TeamGroupAssignDTO>();

            // Player
            CreateMap<Player, PlayerDTO>();

            CreateMap<Player, PlayerDetailDTO>()
            .IncludeBase<Player, PlayerDTO>() // Převezme mapování z PlayerDTO
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            // Coach
            CreateMap<Coach, CoachDTO>();

            CreateMap<Coach, CoachDetailDTO>()
            .IncludeBase<Coach, CoachDTO>() // Převezme mapování z CoachDTO
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : string.Empty))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            // Category
            CreateMap<Category, CategoryDTO>();

            CreateMap<Category, CategoryDetailDTO>()
            .IncludeBase<Category, CategoryDTO>()
            .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.Groups))
            .ForMember(dest => dest.Stats, opt => opt.MapFrom(src => src.Stats))
            .ForMember(dest => dest.Voting, opt => opt.MapFrom(src => src.Voting));

            // Club
            CreateMap<Club, ClubDTO>();

            CreateMap<Club, ClubDetailDTO>()
                .IncludeBase<Club, ClubDTO>()
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams));

            // Event
            CreateMap<Event, EventDTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            // Group
            CreateMap<Group, GroupDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Group, GroupDetailDTO>()
                .IncludeBase<Group, GroupDTO>()
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams))
                .ForMember(dest => dest.Matches, opt => opt.MapFrom(src => src.Matches));

            CreateMap<GroupDTO, Group>();

            CreateMap<GroupDetailDTO, Group>()
                .IncludeBase<GroupDTO, Group>()
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams))
                .ForMember(dest => dest.Matches, opt => opt.MapFrom(src => src.Matches));

            // Match
            CreateMap<Match, MatchDTO>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.ToString()))
                .ForMember(dest => dest.HomeTeam, opt => opt.MapFrom(src => src.HomeTeam))
                .ForMember(dest => dest.AwayTeam, opt => opt.MapFrom(src => src.AwayTeam))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group));


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

            // Tournament
            CreateMap<Tournament, TournamentDTO>()
                .ForMember(dest => dest.Editions, opt => opt.MapFrom(src => src.Editions));

            // TournamentInstance
            CreateMap<TournamentInstance, TournamentInstanceDTO>()
                .ForMember(dest => dest.TournamentName, opt => opt.MapFrom(src => src.Tournament.Name))
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));


            // Person
            CreateMap<Person, PersonDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Login.Username))
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<RegisterDTO, Person>();


            CreateMap<Person, PersonDetailDTO>()
                .IncludeBase<Person, PersonDTO>();

            //// Referee
            //CreateMap<Referee, RefereeDTO>()
            //    .IncludeBase<Person, PersonDTO>();

            //CreateMap<Referee, RefereeDetailDTO>()
            //    .IncludeBase<Referee, RefereeDTO>()
            //    .ForMember(dest => dest.MainRefereeMatchIds, opt => opt.MapFrom(src => src.MainRefereeMatches.Select(m => m.Id)))
            //    .ForMember(dest => dest.AssistantRefereeMatchIds, opt => opt.MapFrom(src => src.AssistantRefereeMatches.Select(m => m.Id)));
        }
    }
}
