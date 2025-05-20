using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface ICategoryRepository
    {
        public Task<IEnumerable<Category>> GetCategoriesAsync(int edition);
        public Task<Category> GetCategoryByIdAsync(int id);
        public Task<Category> AddAsync(Category category);
        public Task<Category> UpdateAsync(Category category);
        public Task<bool> DeleteAsync(int id);
        public Task<List<Category>> GetByTournamentInstanceIdAsync(int tournamentInstanceId);
    }
}
