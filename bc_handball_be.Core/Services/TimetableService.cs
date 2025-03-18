using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class TimetableService : ITimetableService
    {

        private readonly IGroupService _groupService;
        private readonly ITeamService _teamService;
        private readonly IMatchService _matchService;
        private readonly ILogger<TimetableService> _logger;


        private readonly string[] venues = new[] { 
            "Hala", 
            "Hřiště 1 Tartan", 
            "Hřiště 2 Tartan", 
            "Hřiště 3 Umělá tráva", 
            "Hřiště 4 Umělá tráva", 
            "Hřiště 5 Tartan Dělnický dům", 
            "Hřiště 6 Beton Dělnický dům" 
        };

        public TimetableService(IGroupService groupService, ITeamService teamService, IMatchService matchService, ILogger<TimetableService> logger)
        {
            _groupService = groupService;
            _teamService = teamService;
            _matchService = matchService;
            _logger = logger;
        }

        public async Task<List<Match>> GenerateScheduleAsync(int tournamentInstanceId)
        {
            
            throw new NotImplementedException();
        }



    }
}
