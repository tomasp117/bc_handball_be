using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly ILogger<ClubService> _logger;
        public ClubService(IClubRepository clubRepository, ILogger<ClubService> logger)
        {
            _clubRepository = clubRepository;
            _logger = logger;
        }

        public async Task AddClubAsync(Club club)
        {
            await _clubRepository.AddClubAsync(club);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var club = await GetByIdAsync(id);
            if (club == null)
            {
                return false;
            }
            return await _clubRepository.DeleteAsync(club);
        }

        public async Task<List<Club>> GetAllAsync()
        {
            return await _clubRepository.GetAllAsync();
        }

        public async Task<Club> GetByIdAsync(int id)
        {
            return await _clubRepository.GetByIdAsync(id);
        }

        public async Task<Club> UpdateAsync(Club club)
        {
            var existingClub = await GetByIdAsync(club.Id);
            if (existingClub == null)
            {
                return null;
            }
            existingClub.Name = club.Name;
            existingClub.Logo = club.Logo;
            existingClub.Email = club.Email;
            existingClub.Address = club.Address;
            existingClub.State = club.State;
            existingClub.Website = club.Website;

            return await _clubRepository.UpdateAsync(existingClub);

        }
    }
}
