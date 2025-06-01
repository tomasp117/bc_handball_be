using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface IClubAdminRepository
    {
        Task<ClubAdmin> GetByPersonIdAsync(int personId);
        Task<ClubAdmin> GetByClubIdAsync(int clubId);
    }
}
