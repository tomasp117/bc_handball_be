using AutoMapper;
using bc_handball_be.API.DTOs.Clubs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles club management operations including CRUD, logo uploads, and CSV imports.
/// </summary>
[ApiController]
[Route("api/clubs")]
public class ClubController : ControllerBase
{
    private readonly IClubAdminService _clubAdminService;
    private readonly IClubService _clubService;

    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ClubController> _logger;
    private readonly IMapper _mapper;

    public ClubController(IMapper mapper, ILogger<ClubController> logger, IClubService clubService,
        IWebHostEnvironment env, IClubAdminService clubAdminService)
    {
        _mapper = mapper;
        _logger = logger;
        _clubService = clubService;
        _clubAdminService = clubAdminService;
        _env = env;
    }

    /// <summary>
    /// Imports multiple clubs from CSV data (Admin or ClubAdmin only).
    /// </summary>
    /// <param name="clubs">List of clubs to import.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Clubs imported successfully.</response>
    /// <response code="400">If no clubs provided.</response>
    [Authorize(Roles = "Admin, ClubAdmin")]
    [HttpPost("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportClubs([FromBody] List<ClubCsvDTO> clubs)
    {
        try
        {
            _logger.LogInformation("Importing clubs from CSV");
            if (clubs == null || !clubs.Any()) return BadRequest("No clubs to import.");

            foreach (var clubDto in clubs)
            {
                var club = _mapper.Map<Club>(clubDto);
                await _clubService.AddClubAsync(club);
            }

            return Ok("Clubs imported successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing clubs from CSV");
            return StatusCode(500, "An error occurred while importing clubs.");
        }
    }

    /// <summary>
    /// Creates a new club (Admin or ClubAdmin only).
    /// </summary>
    /// <param name="clubDto">The club data.</param>
    /// <returns>The created club.</returns>
    /// <response code="201">Club created successfully.</response>
    /// <response code="400">If club data is invalid.</response>
    [Authorize(Roles = "Admin, ClubAdmin")]
    [HttpPost]
    [ProducesResponseType(typeof(Club), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddClub([FromBody] ClubDTO clubDto)
    {
        try
        {
            _logger.LogInformation("Adding a new club");
            if (clubDto == null) return BadRequest("Club data is required.");
            var club = _mapper.Map<Club>(clubDto);
            await _clubService.AddClubAsync(club);
            return CreatedAtAction(nameof(AddClub), new { id = club.Id }, club);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding new club");
            return StatusCode(500, "An error occurred while adding the club.");
        }
    }

    /// <summary>
    /// Gets all clubs.
    /// </summary>
    /// <returns>List of all clubs.</returns>
    /// <response code="200">Returns the list of clubs.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ClubDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var clubs = await _clubService.GetAllAsync();
            return Ok(_mapper.Map<List<ClubDTO>>(clubs));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all clubs");
            return StatusCode(500, "An error occurred while fetching clubs.");
        }
    }

    /// <summary>
    /// Gets a specific club by ID.
    /// </summary>
    /// <param name="id">The club ID.</param>
    /// <returns>The club details.</returns>
    /// <response code="200">Returns the club.</response>
    /// <response code="404">If club not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClubDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var club = await _clubService.GetByIdAsync(id);
            if (club == null) return NotFound();
            return Ok(_mapper.Map<ClubDTO>(club));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching club with ID {ClubId}", id);
            return StatusCode(500, "An error occurred while fetching the club.");
        }
    }

    /// <summary>
    /// Updates an existing club (Admin only).
    /// </summary>
    /// <param name="id">The club ID.</param>
    /// <param name="dto">The updated club data.</param>
    /// <returns>The updated club.</returns>
    /// <response code="200">Club updated successfully.</response>
    /// <response code="400">If ID mismatch.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] ClubDTO dto)
    {
        try
        {
            if (id != dto.Id) return BadRequest("ID nesouhlasí.");
            var updated = await _clubService.UpdateAsync(_mapper.Map<Club>(dto));
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating club with ID {ClubId}", id);
            return StatusCode(500, "An error occurred while updating the club.");
        }
    }

    /// <summary>
    /// Deletes a club (Admin only).
    /// </summary>
    /// <param name="id">The club ID to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Club deleted successfully.</response>
    /// <response code="404">If club not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _clubService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting club with ID {ClubId}", id);
            return StatusCode(500, "An error occurred while deleting the club.");
        }
    }

    /// <summary>
    /// Uploads a logo image for a club.
    /// </summary>
    /// <param name="clubId">The club ID.</param>
    /// <param name="file">The logo image file (jpg, jpeg, png, or gif).</param>
    /// <returns>The uploaded filename.</returns>
    /// <response code="200">Logo uploaded successfully.</response>
    /// <response code="400">If no file provided or invalid file type.</response>
    /// <response code="500">If file upload fails.</response>
    [HttpPost("{clubId}/upload-logo")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadLogo(int clubId, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                return BadRequest("Nepovolený typ souboru.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Check if the file exists
            if (!System.IO.File.Exists(filePath)) return StatusCode(500, "File upload failed.");


            await _clubService.UpdateLogoAsync(clubId, fileName);

            return Ok($"{fileName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading logo for club {ClubId}", clubId);
            return StatusCode(500, "An error occurred while uploading the logo.");
        }
    }

    /// <summary>
    /// Gets the club administrator for a specific club.
    /// </summary>
    /// <param name="clubId">The club ID.</param>
    /// <returns>The club administrator details.</returns>
    /// <response code="200">Returns the club administrator.</response>
    /// <response code="404">If no administrator found for the club.</response>
    [HttpGet("{clubId}/admin")]
    [ProducesResponseType(typeof(ClubAdminDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClubAdminDTO>> GetClubAdmin(int clubId)
    {
        try
        {
            var admin = await _clubAdminService.GetByClubIdAsync(clubId);
            if (admin == null)
                return NotFound();
            var dto = _mapper.Map<ClubAdminDTO>(admin);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching club admin for club {ClubId}", clubId);
            return StatusCode(500, "An error occurred while fetching the club administrator.");
        }
    }
}