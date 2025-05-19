using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [ApiController]
    [Route("api/clubs")]
    public class ClubController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ClubController> _logger;
        private readonly IClubService _clubService;

        public ClubController(IMapper mapper, ILogger<ClubController> logger, IClubService clubService)
        {
            _mapper = mapper;
            _logger = logger;
            _clubService = clubService;
        }

        [Authorize(Roles = "Admin, ClubAdmin")]
        [HttpPost("import")]
        public async Task<IActionResult> ImportClubs([FromBody] List<ClubCsvDTO> clubs)
        {
            _logger.LogInformation("Importing clubs from CSV");
            if (clubs == null || !clubs.Any())
            {
                return BadRequest("No clubs to import.");
            }

            foreach (var clubDto in clubs)
            {
                var club = _mapper.Map<Club>(clubDto);
                await _clubService.AddClubAsync(club);
            }
            return Ok("Clubs imported successfully.");
        }

    }
}
