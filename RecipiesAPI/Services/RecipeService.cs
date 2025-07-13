using Microsoft.EntityFrameworkCore;
using RecipiesAPI.Data;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly AppDbContext _context;

        private readonly IRecipeCategoryService _categoryService;
        private readonly IRecipeIngredientService _ingredientService;
        private readonly IImageService _imageService;

        public RecipeService(AppDbContext context, IRecipeCategoryService categoryService, IRecipeIngredientService ingredientService, IImageService imageService)
        {
            _context = context;
            _categoryService = categoryService;
            _ingredientService = ingredientService;
            _imageService = imageService;
        }

        public async Task<Recipe> CreateRecipeAsync(CreateRecipeDTO dto)
        {
            // Validate Recipe fields (same as before)
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 500)
                throw new ArgumentException("Name is required and must be less than 500 characters.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException("Description is required.");

            if (dto.CookTime < TimeSpan.Zero)
                throw new ArgumentException("CookTime must be non-negative.");

            if (dto.PrepTime < TimeSpan.Zero)
                throw new ArgumentException("PrepTime must be non-negative.");

            if (dto.Servings < 0)
                throw new ArgumentException("Servings must be non-negative.");

            if (string.IsNullOrWhiteSpace(dto.Instructions))
                throw new ArgumentException("Instructions are required.");

            // Check Author exists
            var authorExists = await _context.Users.AnyAsync(u => u.Id == dto.AuthorId);
            if (!authorExists)
                throw new Exception($"Author with Id {dto.AuthorId} not found.");

            // Create Recipe entity
            var recipe = new Recipe
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                CookTime = dto.CookTime,
                PrepTime = dto.PrepTime,
                Servings = dto.Servings,
                AuthorId = dto.AuthorId,
                Instructions = dto.Instructions.Trim(),
                CreatedAt = DateTime.UtcNow
            };


            try
            {
                _context.Recipes.Add(recipe);

                await _context.SaveChangesAsync();

                await _categoryService.CreateRecipeCategoryAsync(dto.RecipeCategories, recipe.Id);

                await _ingredientService.CreateIngredientAsync(dto.RecipeIngredients, recipe.Id);

                await _imageService.CreateImageAsync(dto.Images, recipe.Id);

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return recipe;

        }
    }
}
