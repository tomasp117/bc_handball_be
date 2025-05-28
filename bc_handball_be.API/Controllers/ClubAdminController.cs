using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Interfaces.IServices;
using bc_handball_be.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bc_handball_be.API.Controllers
{   [Route("api/club-admin")]
    public class ClubAdminController : ControllerBase
    {
        private readonly ILogger<ClubAdminController> _logger;
        private readonly IClubAdminService _clubAdminService;
        private readonly IMapper _mapper;

        public ClubAdminController(ILogger<ClubAdminController> logger, IClubAdminService clubAdminService, IMapper mapper)
        {
            _logger = logger;
            _clubAdminService = clubAdminService;
            _mapper = mapper;
        }

        [HttpGet("my-club")]
        public async Task<IActionResult> GetMyClub()
        {
            _logger.LogInformation("Fetching club details for the current user");
            var personId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var club = await _clubAdminService.GetClubByPersonIdAsync(personId);

            if (club == null)
            {
                _logger.LogWarning("Club not found for person ID {PersonId}", personId);
                return NotFound("Club not found");
            }

            var clubDto = _mapper.Map<ClubDetailDTO>(club);

            return Ok(clubDto);

        }

    }
}
