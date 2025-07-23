using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Services;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDTO dto)
        {
            try
            {
                var createdRecipe = await _recipeService.CreateRecipeAsync(dto);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipeById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid RecipeId. Must be greater than zero." });
            }

            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(id);
                return Ok(recipe);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetRecipesByAuthorId(int authorId)
        {
            if (authorId <= 0)
            {
                return BadRequest(new { message = "Invalid authorId. Must be greater than zero." });
            }

            try
            {
                var recipes = await _recipeService.GetRecipesByAuthorIdAsync(authorId);
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRecipes()
        {
            try
            {
                var recipes = await _recipeService.GetAllRecipesAsync();
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetRecipesByCategoryId(int categoryId)
        {
            if (categoryId <= 0)
            {
                return BadRequest(new { message = "Invalid categoryId. Must be greater than zero." });
            }

            try
            {
                var recipes = await _recipeService.GetRecipesByCategoryIdAsync(categoryId);
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
