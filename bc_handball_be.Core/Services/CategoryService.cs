using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            this._categoryRepository = categoryRepository;
            this.logger = logger;
        }


        public async Task<IEnumerable<Category>> GetCategoriesAsync(int edition)
        {
            return await _categoryRepository.GetCategoriesAsync(edition);               
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetCategoryByIdAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            if (category == null)
            {
                logger.LogError("Category is null");
                throw new ArgumentNullException(nameof(category));
            }
            return await _categoryRepository.AddAsync(category);
        }

    }
}
