using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMapper _mapper;

        public MatchController(IMapper mapper)
        {
            this._mapper = mapper;

        }

        [HttpGet("id")]
        public ActionResult<MatchDTO> GetMatch(int id)
        {
            var testMatch = new Match
            {
                Id = id,
                Time = DateTime.Now,
                TimePlayed = "15:30",
                Playground = "Hala A",
                Score = "25:22",
                State = MatchState.Pending,
                HomeTeam = new Team { Id = 1, Name = "Team A" },
                AwayTeam = new Team { Id = 2, Name = "Team B" },
                Group = new Group { Id = 1, Name = "Group A" }
            };

            var dto = _mapper.Map<MatchDTO>(testMatch);
            return Ok(dto);
        }

        [HttpGet]
        public ActionResult<List<MatchDTO>> GetTestMatches()
        {
            var testMatches = new List<Match>
            {
                new Match
                {
                    Id = 1,
                    Time = DateTime.Now,
                    TimePlayed = "15:30",
                    Playground = "Hala A",
                    Score = "25:22",
                    State = MatchState.Pending,
                    HomeTeam = new Team { Id = 1, Name = "Team A" },
                    AwayTeam = new Team { Id = 2, Name = "Team B" },
                    Group = new Group { Id = 1, Name = "Group A" }
                },
                new Match
                {
                    Id = 2,
                    Time = DateTime.Now.AddHours(2),
                    TimePlayed = "18:00",
                    Playground = "Hala B",
                    Score = "30:28",
                    State = MatchState.Done,
                    HomeTeam = new Team { Id = 3, Name = "Team C" },
                    AwayTeam = new Team { Id = 4, Name = "Team D" },
                    Group = new Group { Id = 2, Name = "Group B" }
                }
            };

            var dtos = _mapper.Map<List<MatchDTO>>(testMatches);
            return Ok(dtos);
        }
    }
}