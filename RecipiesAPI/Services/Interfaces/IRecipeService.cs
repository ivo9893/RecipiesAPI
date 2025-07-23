using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Models.DTO.Responce;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<Recipe> CreateRecipeAsync(CreateRecipeDTO dto);
        Task<RecipeResponse> GetRecipeByIdAsync(int id);
        Task<List<RecipeResponse>> GetRecipesByAuthorIdAsync(int authorId);
        Task<List<RecipeResponse>> GetAllRecipesAsync();
        Task<List<RecipeResponse>> GetRecipesByCategoryIdAsync(int categoryId);
    }
}
