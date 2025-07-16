using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IImageService
    {
        Task<List<Image>> CreateImageAsync(List<CreateImageDTO> images, int recipeId = -1);
    }
}
