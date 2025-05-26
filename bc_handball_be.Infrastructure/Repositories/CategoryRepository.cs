using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(ApplicationDbContext context, ILogger<CategoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync(int edition)
        {
            _logger.LogInformation("Fetching categories");
            var categories = await _context.Categories
                .Where(c => c.TournamentInstance.EditionNumber == edition)
                .ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Fetching category with id {id}", id);
            var category = await _context.Categories.Include(c => c.TournamentInstance).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                _logger.LogWarning("Category with id {id} not found", id);
                return null;
            }
            return category;
        }

        public async Task<Category> AddAsync(Category category)
        {
            _logger.LogInformation("Adding category {category}", category);
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _logger.LogInformation("Updating category {category}", category);
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting category with id {id}", id);
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with id {id} not found", id);
                return false;
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetByTournamentInstanceIdAsync(int tournamentInstanceId)
        {
            _logger.LogInformation("Fetching categories for tournament instance id {tournamentInstanceId}", tournamentInstanceId);
            var categories = await _context.Categories
                .Where(c => c.TournamentInstanceId == tournamentInstanceId)
                .ToListAsync();
            return categories;
        }
    }
}
