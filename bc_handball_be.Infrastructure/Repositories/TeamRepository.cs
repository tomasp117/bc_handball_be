﻿using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace bc_handball_be.Infrastructure.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TeamRepository> _logger;


        public TeamRepository(ApplicationDbContext context, ILogger<TeamRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddTeamAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeamAsync(int id)
        {
            await _context.Teams
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.Category)
                .Include(t => t.TeamGroups)
                    .ThenInclude(tg => tg.Group)
                .Include(t => t.Players)
                    .ThenInclude(p => p.Person)
                .Include(t => t.Club)        
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Team>> GetTeamsAsync()
        {
            return await _context.Teams.Include(t => t.Category).ToListAsync();
        }

        public async Task<List<Team>> GetTeamsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Fetching teams for categoryId: {CategoryId}", categoryId);
            try
            {
                var teams = await _context.Teams.Where(team => team.CategoryId == categoryId).ToListAsync();
                if (!teams.Any())
                {
                    _logger.LogWarning("No teams found for the given category: {CategoryId}", categoryId);
                }
                return teams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams for categoryId: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<List<Team>> GetTeamsByGroupAsync(int groupId)
        {
            return await _context.Teams
                .Include(t => t.Category)
                .Include(t => t.TeamGroups)
                .Where(t => t.TeamGroups.Any(tg => tg.GroupId == groupId))
                .ToListAsync();
        }

        public async Task<List<Team>> GetTeamsByIdAsync(List<int> ids)
        {
            _logger.LogInformation("Fetching teams by Ids: {Ids}", ids);
            try
            {
                var teams = await _context.Teams.Where(team => ids.Contains(team.Id)).ToListAsync();
                if (!teams.Any())
                {
                    _logger.LogWarning("No teams found for the given Ids: {Ids}", ids);
                }
                return teams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams by Ids: {Ids}", ids);
                throw;
            }
        }

        public Task UpdateTeamAsync(Team team)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Team>> GetPlaceholderTeamsByCategoryAsync(int categoryId)
        {
            return await _context.Teams
                .Where(t => t.IsPlaceholder == true && t.CategoryId == categoryId)
                .ToListAsync();
        }

    }
}
