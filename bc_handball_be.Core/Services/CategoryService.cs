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
                return null!;
            }
            return await _categoryRepository.AddAsync(category);
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            if (category == null)
            {
                logger.LogError("Category is null");
                return null!;
            }
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(category.Id);
            if (existingCategory == null)
            {
                logger.LogError("Category with ID {Id} not found", category.Id);
                return null!;
            }
            existingCategory.Name = category.Name;


            return await _categoryRepository.UpdateAsync(existingCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                logger.LogError("Category with ID {Id} not found", id);
                return false;
            }
            return await _categoryRepository.DeleteAsync(id);
        }

        public async Task<List<Category>> GetByTournamentInstanceIdAsync(int tournamentInstanceId)
        {
            var categories = await _categoryRepository.GetByTournamentInstanceIdAsync(tournamentInstanceId);
            return categories.ToList();
        }

        public async Task<Category?> GetByNameAsync(string name, int tournamentInstanceId)
        {
            var categories = await _categoryRepository.GetByTournamentInstanceIdAsync(tournamentInstanceId);
            return categories.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

    }
}
