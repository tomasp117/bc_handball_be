using System.Security.Claims;
using AutoMapper;
using bc_handball_be.API.DTOs.Clubs;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles club administrator operations including account management and club access.
/// </summary>
[Route("api/club-admin")]
[ApiController]
public class ClubAdminController : ControllerBase
{
    private readonly IClubAdminService _clubAdminService;
    private readonly ILogger<ClubAdminController> _logger;
    private readonly IMapper _mapper;

    public ClubAdminController(ILogger<ClubAdminController> logger, IClubAdminService clubAdminService, IMapper mapper)
    {
        _logger = logger;
        _clubAdminService = clubAdminService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets the club details for the currently logged-in club administrator.
    /// </summary>
    /// <returns>Detailed club information.</returns>
    /// <response code="200">Returns the club details.</response>
    /// <response code="404">If no club found for the current user.</response>
    [HttpGet("my-club")]
    [ProducesResponseType(typeof(ClubDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyClub()
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching club for current user");
            return StatusCode(500, "An error occurred while fetching club details.");
        }
    }

    /// <summary>
    /// Creates a new club administrator account (Admin only).
    /// </summary>
    /// <param name="dto">Club administrator registration data.</param>
    /// <returns>The created club administrator.</returns>
    /// <response code="200">Club administrator created successfully.</response>
    /// <response code="400">If username already exists or data is invalid.</response>
    /// <response code="500">If an error occurred during creation.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ClubAdminDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateClubAdmin([FromBody] ClubAdminCreateDTO dto)
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