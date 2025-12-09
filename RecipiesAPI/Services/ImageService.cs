using Microsoft.EntityFrameworkCore;
using RecipiesAPI.Data;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Services
{
    public class ImageService : IImageService
    {
        private readonly AppDbContext _context;
        public ImageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Image>> CreateImageAsync(List<CreateImageDTO> images, int recipeId)
        {

            var imageList = new List<Image>();  

            foreach (var image in images)
            {
                if (string.IsNullOrEmpty(image.Url))
                {
                    throw new ArgumentException("Image URL must be provided.");
                }
                var imageObj = new Image
                {
                    RecipeId = recipeId,
                    Url = image.Url
                };
                imageList.Add(imageObj);
            }

      

            _context.Images.AddRange(imageList);
            await _context.SaveChangesAsync();

            return imageList;
        }
    }
}
