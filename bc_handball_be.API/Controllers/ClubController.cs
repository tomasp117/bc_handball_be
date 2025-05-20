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
    }
}
