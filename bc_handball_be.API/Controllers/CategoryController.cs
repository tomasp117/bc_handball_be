using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{

    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger, IMapper mapper)
        {
            _categoryService = categoryService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();

                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found.");
                    return NotFound("No categories available.");
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return StatusCode(500, "An error occurred while fetching categories.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDetailDTO>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found.", id);
                    return NotFound($"Category with ID {id} not found.");
                }

                var categoryDto = _mapper.Map<CategoryDetailDTO>(category);

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category with ID {Id}", id);
                return StatusCode(500, "An error occurred while fetching the category.");
            }
        }


    }
}
