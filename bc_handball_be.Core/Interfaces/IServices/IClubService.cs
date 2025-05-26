using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface IClubService
    {
        Task AddClubAsync(Club club);
        Task<Club> GetByIdAsync(int id);
        Task<List<Club>> GetAllAsync();
        Task<Club> UpdateAsync(Club club);
        Task<bool> DeleteAsync(int id);
        Task<Club> GetPlaceholderClubAsync();
    }
}
