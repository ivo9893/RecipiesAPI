using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipiesAPI.Data;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Services;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly AppDbContext _context;
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(IRecipeService recipeService, AppDbContext context, ILogger<RecipeController> logger)
        {
            _recipeService = recipeService;
            _context = context;
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdRecipe = await _recipeService.CreateRecipeAsync(dto);
                return Ok("Successfully created recipe.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("insert")]
        public async Task<IActionResult> CreateRecipies([FromBody] List<CreateRecipeDTO> dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto == null || dto.Count == 0)
            {
                return BadRequest(new { message = "No recipes provided." });
            }

            _logger.LogInformation("Starting bulk insert of {Count} recipes", dto.Count);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var createdCount = 0;
                foreach(var recipe in dto)
                {
                    await _recipeService.CreateRecipeAsync(recipe);
                    createdCount++;
                }

                await transaction.CommitAsync();

                _logger.LogInformation("Successfully bulk inserted {Count} recipes", createdCount);
                return Ok(new { message = $"Successfully created {createdCount} recipes.", count = createdCount });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to bulk insert recipes. Transaction rolled back.");
                return BadRequest(new { message = "An error occurred while creating recipes. All changes have been rolled back." });
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

        [HttpGet("paged")]
        public async Task<IActionResult> GetAllRecipesPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var pagedRecipes = await _recipeService.GetAllRecipesPagedAsync(pageNumber, pageSize);
                return Ok(pagedRecipes);
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
