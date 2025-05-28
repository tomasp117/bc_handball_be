using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bc_handball_be.API.Controllers
{   [Route("api/club-admin")]
    public class ClubAdminController : ControllerBase
    {
        private readonly ILogger<ClubAdminController> _logger;
        private readonly IClubAdminService _clubAdminService;

        public ClubAdminController(ILogger<ClubAdminController> logger, IClubAdminService clubAdminService)
        {
            _logger = logger;
            _clubAdminService = clubAdminService;
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

            return Ok(club);

        }



    }
}
