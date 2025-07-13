using RecipiesAPI.Models.DTO;
using RecipiesAPI.Models;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IRecipeCategoryService
    {
        Task<List<RecipeCategory>> CreateRecipeCategoryAsync(List<CreateRecipeCategoryDTO> categories, int recipeId = -1);
    }
}
