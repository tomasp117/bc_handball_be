using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{

    //[Route("api/{edition}/categories")]
    [Route("api")]
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

        [HttpGet("{edition}/categories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories(int edition)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync(edition);

                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found for edition {Edition}", edition);
                    return NotFound("No categories available.");
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories for edition {Edition}", edition);
                return StatusCode(500, "An error occurred while fetching categories.");
            }
        }

        [HttpGet("categories/{id}")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryPostDTO categoryDto)
        {
            if (categoryDto == null)
            {
                _logger.LogWarning("Received null category object.");
                return BadRequest("Category data is required.");
            }
            try
            {

                var category = _mapper.Map<Category>(categoryDto);
                var createdCategory = await _categoryService.CreateCategoryAsync(category);
                var createdCategoryDto = _mapper.Map<CategoryPostDTO>(createdCategory);

                return Ok(createdCategoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, "An error occurred while creating the category.");
            }
        }


    }
}
