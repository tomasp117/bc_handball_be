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
    }
}
