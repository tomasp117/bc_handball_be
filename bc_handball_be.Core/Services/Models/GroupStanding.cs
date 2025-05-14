using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services.Models
{
    public class GroupStanding
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;

        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }

        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }

        public int Points { get; set; }
    }
}
