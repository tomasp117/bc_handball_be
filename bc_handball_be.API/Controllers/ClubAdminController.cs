using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities.Actors.sub;
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateClubAdmin([FromBody] CreateClubAdminDTO dto)
        {
            try
            {
                var clubAdmin = _mapper.Map<ClubAdmin>(dto);

                // DŮLEŽITÉ: Nastavit heslo hashovaně až teď!
                clubAdmin.Person.Login.SetPassword(dto.Password);

                await _clubAdminService.CreateAsync(clubAdmin);
                return Ok(_mapper.Map<ClubAdminDTO>(clubAdmin));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Username conflict when creating club admin.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při vytváření ClubAdmina.");
                return StatusCode(500, "Něco se pokazilo při ukládání ClubAdmina.");
            }
        }
    }
}
