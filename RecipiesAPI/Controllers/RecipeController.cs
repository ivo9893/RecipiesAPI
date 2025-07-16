using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipiesAPI.Models.DTO;
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
    }
}
