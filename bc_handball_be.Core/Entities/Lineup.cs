using bc_handball_be.Core.Entities.IdentityField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Entities
{
    public class Lineup : BaseEntity
    {
        public int MatchId { get; set; }
        public int TeamId { get; set; }

        public Match Match { get; set; } = null!;
        public Team Team { get; set; } = null!;

        public List<LineupPlayer> Players { get; set; } = new();
    }
}
