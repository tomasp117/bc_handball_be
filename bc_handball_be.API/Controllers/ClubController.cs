using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using bc_handball_be.Core.Services;
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
        private readonly IClubAdminService _clubAdminService;

        private readonly IWebHostEnvironment _env;

        public ClubController(IMapper mapper, ILogger<ClubController> logger, IClubService clubService, IWebHostEnvironment env, IClubAdminService clubAdminService)
        {
            _mapper = mapper;
            _logger = logger;
            _clubService = clubService;
            _clubAdminService = clubAdminService;
            _env = env;
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

        [Authorize(Roles = "Admin, ClubAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddClub([FromBody] ClubDTO clubDto)
        {
            _logger.LogInformation("Adding a new club");
            if (clubDto == null)
            {
                return BadRequest("Club data is required.");
            }
            var club = _mapper.Map<Club>(clubDto);
            await _clubService.AddClubAsync(club);
            return CreatedAtAction(nameof(AddClub), new { id = club.Id }, club);

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clubs = await _clubService.GetAllAsync();
            return Ok(_mapper.Map<List<ClubDTO>>(clubs));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var club = await _clubService.GetByIdAsync(id);
            if (club == null) return NotFound();
            return Ok(_mapper.Map<ClubDTO>(club));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClubDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID nesouhlasí.");
            var updated = await _clubService.UpdateAsync(_mapper.Map<Club>(dto));
            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _clubService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("{clubId}/upload-logo")]
        public async Task<IActionResult> UploadLogo(int clubId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            {
                return BadRequest("Nepovolený typ souboru.");
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return StatusCode(500, "File upload failed.");
            }


            await _clubService.UpdateLogoAsync(clubId, fileName);

            return Ok($"{fileName}");
        }

        [HttpGet("{clubId}/admin")]
        public async Task<ActionResult<ClubAdminDTO>> GetClubAdmin(int clubId)
        {
            var admin = await _clubAdminService.GetByClubIdAsync(clubId);
            if (admin == null)
                return NotFound();
            var dto = _mapper.Map<ClubAdminDTO>(admin);
            return Ok(dto);
        }
    }
}
