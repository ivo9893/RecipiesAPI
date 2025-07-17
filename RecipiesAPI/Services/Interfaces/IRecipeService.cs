using RecipiesAPI.Models.DTO;
using RecipiesAPI.Models;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<Recipe> CreateRecipeAsync(CreateRecipeDTO dto);
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<List<Recipe>> GetRecipesByAuthorIdAsync(int authorId);
        Task<List<Recipe>> GetAllRecipesAsync();
    }
}
