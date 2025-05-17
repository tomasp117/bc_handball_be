using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync(int edition);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
    }
}
