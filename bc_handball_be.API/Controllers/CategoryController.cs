using AutoMapper;
using bc_handball_be.API.DTOs.Categories;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles category management operations including CRUD and tournament instance associations.
/// </summary>
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

    /// <summary>
    /// Gets all categories for a specific tournament edition.
    /// </summary>
    /// <param name="edition">The tournament edition number.</param>
    /// <returns>List of categories for the edition.</returns>
    /// <response code="200">Returns the list of categories.</response>
    /// <response code="404">If no categories found for the edition.</response>
    [HttpGet("{edition}/categories")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories(int edition)
    {
        var categories = await _categoryService.GetCategoriesAsync(edition);

        if (categories == null || !categories.Any())
        {
            _logger.LogWarning("No categories found for edition {Edition}", edition);
            return NotFound("No categories available.");
        }

        return Ok(categories);
    }

    /// <summary>
    /// Gets detailed information about a specific category.
    /// </summary>
    /// <param name="id">The category ID.</param>
    /// <returns>Detailed category information.</returns>
    /// <response code="200">Returns the category details.</response>
    /// <response code="404">If category not found.</response>
    [HttpGet("categories/{id}")]
    [ProducesResponseType(typeof(CategoryDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDetailDTO>> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Category with ID {Id} not found.", id);
            return NotFound($"Category with ID {id} not found.");
        }

        var categoryDto = _mapper.Map<CategoryDetailDTO>(category);

        return Ok(categoryDto);
    }

    /// <summary>
    /// Creates a new category (Admin only).
    /// </summary>
    /// <param name="categoryDto">The category data.</param>
    /// <returns>The created category.</returns>
    /// <response code="200">Category created successfully.</response>
    /// <response code="400">If category data is invalid.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("categories")]
    [ProducesResponseType(typeof(CategoryCreateDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO categoryDto)
    {
        if (categoryDto == null)
        {
            _logger.LogWarning("Received null category object.");
            return BadRequest("Category data is required.");
        }

        var category = _mapper.Map<Category>(categoryDto);
        var createdCategory = await _categoryService.CreateCategoryAsync(category);
        var createdCategoryDto = _mapper.Map<CategoryCreateDTO>(createdCategory);

        return Ok(createdCategoryDto);
    }

    /// <summary>
    /// Updates an existing category (Admin only).
    /// </summary>
    /// <param name="id">The category ID.</param>
    /// <param name="dto">The updated category data.</param>
    /// <returns>The updated category.</returns>
    /// <response code="200">Category updated successfully.</response>
    /// <response code="400">If ID mismatch.</response>
    /// <response code="404">If category not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("categories/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryDTO dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        var updated = await _categoryService.UpdateCategoryAsync(_mapper.Map<Category>(dto));
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a category (Admin only).
    /// </summary>
    /// <param name="id">The category ID to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Category deleted successfully.</response>
    /// <response code="404">If category not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("categories/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Gets all categories for a specific tournament instance.
    /// </summary>
    /// <param name="instanceId">The tournament instance ID.</param>
    /// <returns>List of categories for the instance.</returns>
    /// <response code="200">Returns the list of categories.</response>
    [HttpGet("categories/by-instance")]
    [ProducesResponseType(typeof(List<CategoryDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByInstance([FromQuery] int instanceId)
    {
        var categories = await _categoryService.GetByTournamentInstanceIdAsync(instanceId);
        var dto = _mapper.Map<List<CategoryDTO>>(categories);
        return Ok(dto);
    }
}