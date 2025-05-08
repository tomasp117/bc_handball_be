using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Entities
{
    public class TeamGroup
    {
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;


    }
}
