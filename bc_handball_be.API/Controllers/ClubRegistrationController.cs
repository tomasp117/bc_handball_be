using AutoMapper;
using bc_handball_be.API.DTOs.Registrations;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles club registration operations for tournament instances.
/// </summary>
[ApiController]
[Route("api/registrations")]
public class ClubRegistrationController : ControllerBase
{
    private readonly IClubRegistrationService _registrationService;
    private readonly ILogger<ClubRegistrationController> _logger;
    private readonly IMapper _mapper;

    public ClubRegistrationController(
        IClubRegistrationService registrationService,
        ILogger<ClubRegistrationController> logger,
        IMapper mapper
    )
    {
        _registrationService = registrationService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new club registration (public endpoint for new clubs).
    /// This creates: Club, Person, Login, ClubAdmin, and ClubRegistration in one transaction.
    /// </summary>
    /// <param name="registrationDto">The registration data including club and admin information.</param>
    /// <returns>The created registration.</returns>
    /// <response code="201">Registration created successfully.</response>
    /// <response code="400">If registration data is invalid.</response>
    [AllowAnonymous] // Public endpoint - no authentication required for club registration
    [HttpPost]
    [ProducesResponseType(typeof(ClubRegistrationDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRegistration(
        [FromBody] ClubRegistrationCreateDTO registrationDto
    )
    {
        try
        {
            _logger.LogInformation(
                "Creating new club registration with club and admin information"
            );
            if (registrationDto == null)
                return BadRequest("Registration data is required.");

            // Validate required nested objects
            if (registrationDto.Club == null)
                return BadRequest("Club information is required.");
            if (registrationDto.ClubAdmin == null)
                return BadRequest("Club admin information is required.");

            // Map DTOs to entities
            var club = _mapper.Map<Club>(registrationDto.Club);
            var person = _mapper.Map<Person>(registrationDto.ClubAdmin);
            var registrationData = new ClubRegistration
            {
                TournamentInstanceId = registrationDto.TournamentInstanceId,
                PackageACount = registrationDto.PackageACount,
                PackageBCount = registrationDto.PackageBCount,
                CategoryTeamCounts = _mapper.Map<List<ClubRegistrationCategory>>(
                    registrationDto.CategoryTeamCounts
                ),
            };

            // Create the full registration (Club + Person + Login + ClubAdmin + Registration)
            var created = await _registrationService.CreateFullRegistrationAsync(
                club,
                person,
                registrationDto.ClubAdmin.Username,
                registrationDto.ClubAdmin.Password,
                registrationData
            );

            var resultDto = _mapper.Map<ClubRegistrationDTO>(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, resultDto);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation during club registration");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating club registration");
            return StatusCode(500, "An error occurred while creating the registration.");
        }
    }

    /// <summary>
    /// Gets all club registrations.
    /// </summary>
    /// <returns>List of all registrations.</returns>
    /// <response code="200">Returns the list of registrations.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(typeof(List<ClubRegistrationDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var registrations = await _registrationService.GetAllAsync();
            return Ok(_mapper.Map<List<ClubRegistrationDTO>>(registrations));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all club registrations");
            return StatusCode(500, "An error occurred while fetching registrations.");
        }
    }

    /// <summary>
    /// Gets a specific club registration by ID.
    /// </summary>
    /// <param name="id">The registration ID.</param>
    /// <returns>The registration details.</returns>
    /// <response code="200">Returns the registration.</response>
    /// <response code="404">If registration not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClubRegistrationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var registration = await _registrationService.GetByIdAsync(id);
            if (registration == null)
                return NotFound();
            return Ok(_mapper.Map<ClubRegistrationDTO>(registration));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching club registration with ID {Id}", id);
            return StatusCode(500, "An error occurred while fetching the registration.");
        }
    }

    /// <summary>
    /// Gets all registrations for a specific tournament instance.
    /// </summary>
    /// <param name="tournamentInstanceId">The tournament instance ID.</param>
    /// <returns>List of registrations for the tournament.</returns>
    /// <response code="200">Returns the list of registrations.</response>
    [HttpGet("tournament/{tournamentInstanceId}")]
    [ProducesResponseType(typeof(List<ClubRegistrationDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTournamentInstanceId(int tournamentInstanceId)
    {
        try
        {
            var registrations = await _registrationService.GetByTournamentInstanceIdAsync(
                tournamentInstanceId
            );
            return Ok(_mapper.Map<List<ClubRegistrationDTO>>(registrations));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error fetching registrations for tournament instance {TournamentInstanceId}",
                tournamentInstanceId
            );
            return StatusCode(500, "An error occurred while fetching registrations.");
        }
    }

    /// <summary>
    /// Gets registration for a specific club.
    /// </summary>
    /// <param name="clubId">The club ID.</param>
    /// <returns>The club's registration.</returns>
    /// <response code="200">Returns the registration.</response>
    /// <response code="404">If no registration found for the club.</response>
    [HttpGet("club/{clubId}")]
    [ProducesResponseType(typeof(ClubRegistrationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByClubId(int clubId)
    {
        try
        {
            var registration = await _registrationService.GetByClubIdAsync(clubId);
            if (registration == null)
                return NotFound();
            return Ok(_mapper.Map<ClubRegistrationDTO>(registration));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching registration for club {ClubId}", clubId);
            return StatusCode(500, "An error occurred while fetching the registration.");
        }
    }

    /// <summary>
    /// Gets all registrations with a specific status.
    /// </summary>
    /// <param name="status">The registration status (Pending, Approved, Denied).</param>
    /// <returns>List of registrations with the specified status.</returns>
    /// <response code="200">Returns the list of registrations.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(List<ClubRegistrationDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStatus(string status)
    {
        try
        {
            if (!Enum.TryParse<RegistrationStatus>(status, true, out var registrationStatus))
            {
                return BadRequest("Invalid status value.");
            }

            var registrations = await _registrationService.GetByStatusAsync(registrationStatus);
            return Ok(_mapper.Map<List<ClubRegistrationDTO>>(registrations));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching registrations with status {Status}", status);
            return StatusCode(500, "An error occurred while fetching registrations.");
        }
    }

    /// <summary>
    /// Updates an existing club registration.
    /// </summary>
    /// <param name="id">The registration ID.</param>
    /// <param name="updateDto">The updated registration data.</param>
    /// <returns>The updated registration.</returns>
    /// <response code="200">Registration updated successfully.</response>
    /// <response code="404">If registration not found.</response>
    [Authorize(Roles = "Admin, ClubAdmin")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClubRegistrationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRegistration(
        int id,
        [FromBody] ClubRegistrationUpdateDTO updateDto
    )
    {
        try
        {
            var registration = await _registrationService.GetByIdAsync(id);
            if (registration == null)
                return NotFound();

            registration.PackageACount = updateDto.PackageACount;
            registration.PackageBCount = updateDto.PackageBCount;
            registration.CategoryTeamCounts = _mapper.Map<List<ClubRegistrationCategory>>(
                updateDto.CategoryTeamCounts
            );

            var updated = await _registrationService.UpdateRegistrationAsync(registration);
            return Ok(_mapper.Map<ClubRegistrationDTO>(updated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating club registration with ID {Id}", id);
            return StatusCode(500, "An error occurred while updating the registration.");
        }
    }

    /// <summary>
    /// Approves a club registration (Admin only).
    /// </summary>
    /// <param name="id">The registration ID.</param>
    /// <returns>The approved registration.</returns>
    /// <response code="200">Registration approved successfully.</response>
    /// <response code="404">If registration not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(ClubRegistrationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveRegistration(int id)
    {
        try
        {
            var registration = await _registrationService.ApproveRegistrationAsync(id);
            if (registration == null)
                return NotFound();
            return Ok(_mapper.Map<ClubRegistrationDTO>(registration));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving club registration with ID {Id}", id);
            return StatusCode(500, "An error occurred while approving the registration.");
        }
    }

    /// <summary>
    /// Denies a club registration (Admin only).
    /// </summary>
    /// <param name="id">The registration ID.</param>
    /// <param name="denialReason">The reason for denial.</param>
    /// <returns>The denied registration.</returns>
    /// <response code="200">Registration denied successfully.</response>
    /// <response code="400">If denial reason is missing.</response>
    /// <response code="404">If registration not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/deny")]
    [ProducesResponseType(typeof(ClubRegistrationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DenyRegistration(int id, [FromBody] string denialReason)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(denialReason))
                return BadRequest("Denial reason is required.");

            var registration = await _registrationService.DenyRegistrationAsync(id, denialReason);
            if (registration == null)
                return NotFound();
            return Ok(_mapper.Map<ClubRegistrationDTO>(registration));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error denying club registration with ID {Id}", id);
            return StatusCode(500, "An error occurred while denying the registration.");
        }
    }

    /// <summary>
    /// Deletes a club registration (Admin only).
    /// </summary>
    /// <param name="id">The registration ID to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Registration deleted successfully.</response>
    /// <response code="404">If registration not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRegistration(int id)
    {
        try
        {
            var result = await _registrationService.DeleteRegistrationAsync(id);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting club registration with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the registration.");
        }
    }
}
