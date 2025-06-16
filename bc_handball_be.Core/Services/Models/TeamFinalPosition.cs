using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services.Models
{
    public class TeamFinalPosition
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = "";
        public int FinalPlace { get; set; }
    }
}
