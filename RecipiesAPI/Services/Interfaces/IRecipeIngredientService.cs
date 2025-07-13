using RecipiesAPI.Models.DTO;
using RecipiesAPI.Models;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IRecipeIngredientService
    {
        Task<List<RecipeIngredient>> CreateIngredientAsync(List<CreateRecipeIngredientDTO> dto, int recipeId = -1);
    }
}
