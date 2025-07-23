using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IRecipeIngredientService
    {
        Task<List<RecipeIngredient>> CreateIngredientAsync(List<CreateRecipeIngredientDTO> dto, int recipeId = -1);
    }
}
