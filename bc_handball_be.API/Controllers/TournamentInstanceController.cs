﻿using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [Route("api/tournament-instances")]
    public class TournamentInstanceController : ControllerBase
    {
        private readonly ILogger<TournamentInstanceController> _logger;
        private readonly IMapper _mapper;
        private readonly ITournamentInstanceService _tournamentInstanceService;

        public TournamentInstanceController(ILogger<TournamentInstanceController> logger, IMapper mapper, ITournamentInstanceService tournamentInstanceService)
        {
            _logger = logger;
            _mapper = mapper;
            _tournamentInstanceService = tournamentInstanceService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateInstance([FromBody] TournamentInstanceDTO dto)
        {
            var tournamentInstance = _mapper.Map<TournamentInstance>(dto);
            var created = await _tournamentInstanceService.CreateTournamentInstanceAsync(tournamentInstance);
            return Ok(created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInstances()
        {
            var tournamentInstances = await _tournamentInstanceService.GetAllTournamentInstancesAsync();
            var tournamentInstancesDto = _mapper.Map<List<TournamentInstanceDTO>>(tournamentInstances);
            return Ok(tournamentInstancesDto);
        }

        [HttpGet("by-tournament")]
        public async Task<IActionResult> GetByTournamentId([FromQuery] int tournamentId)
        {
            var instances = await _tournamentInstanceService.GetByTournamentIdAsync(tournamentId);
            var instancesDto = _mapper.Map<List<TournamentInstanceDTO>>(instances);
            return Ok(instancesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var instance = await _tournamentInstanceService.GetByIdAsync(id);
            if (instance == null)
                return NotFound();

            var dto = _mapper.Map<TournamentInstanceDTO>(instance);
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TournamentInstanceDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var instance = _mapper.Map<TournamentInstance>(dto);
            var updated = await _tournamentInstanceService.UpdateTournamentInstanceAsync(instance);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var instance = await _tournamentInstanceService.GetByIdAsync(id);
            if (instance == null)
                return NotFound();
            await _tournamentInstanceService.DeleteTournamentInstanceAsync(id);
            return NoContent();
        }
    }
}
