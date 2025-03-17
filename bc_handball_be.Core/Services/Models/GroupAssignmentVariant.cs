using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services.Models
{
    public class GroupAssignmentVariant
    {
        public int GroupCount { get; set; }
        public int TotalMatches { get; set; }
        public double MinMatchesPerTeam { get; set; }
        public List<Group> Groups { get; set; } = new();
    }
}
