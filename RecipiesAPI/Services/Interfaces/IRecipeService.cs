using RecipiesAPI.Models.DTO;
using RecipiesAPI.Models;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<Recipe> CreateRecipeAsync(CreateRecipeDTO dto);
    }
}
