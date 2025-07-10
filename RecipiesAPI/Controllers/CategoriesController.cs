using Microsoft.AspNetCore.Mvc;
using RecipiesAPI.Models.DTO;
using RecipiesAPI.Models;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // e.g., /api/categories
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="categoryDto">The category data to create.</param>
        /// <returns>The newly created category.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Category), 201)] 
        [ProducesResponseType(400)] 
        [ProducesResponseType(409)] 
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);        
                return CreatedAtAction(nameof(CreateCategory), new { id = createdCategory.Id }, createdCategory);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the category.");
            }
        }
    }
}
