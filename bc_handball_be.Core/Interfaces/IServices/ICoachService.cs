using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface ICoachService
    {
        Task<Team?> GetTeamForPersonIdAsync(int personId);
    }
}
