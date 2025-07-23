using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IRecipeCategoryService
    {
        Task<List<RecipeCategory>> CreateRecipeCategoryAsync(List<CreateRecipeCategoryDTO> categories, int recipeId = -1);
    }
}
