using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;

namespace RecipiesAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategoryAsync(CreateCategoryDTO categoryDTO);

    }
}
