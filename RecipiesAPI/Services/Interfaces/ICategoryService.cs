using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Models.DTO.Responce;

namespace RecipiesAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryDTO categoryDTO);
        Task<List<CategoryResponse>> GetAllCategoriesAsync();

    }
}
