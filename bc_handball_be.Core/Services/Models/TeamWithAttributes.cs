using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services.Models
{
    public class TeamWithAttributes
    {
        public Team Team { get; }
        public int Strength { get; }
        public bool IsGirls { get; }

        public TeamWithAttributes(Team team, int strength, bool isGirls)
        {
            Team = team ?? throw new ArgumentNullException(nameof(team));
            Strength = strength;
            IsGirls = isGirls;
        }
    }
}
