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
        private readonly ICategoryRepository categoryRepository;
        private readonly ILogger<CategoryService> logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            this.categoryRepository = categoryRepository;
            this.logger = logger;
        }


        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await categoryRepository.GetCategoriesAsync();   
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await categoryRepository.GetCategoryByIdAsync(id);
        }

    }
}
