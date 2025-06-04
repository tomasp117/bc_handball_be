using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.IdentityField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Entities
{
    public class LineupPlayer : BaseEntity
    {

        public int LineupId { get; set; }
        public int PlayerId { get; set; }

        public Player Player { get; set; } = null!;
        public Lineup Lineup { get; set; } = null!;
        
    }
}
