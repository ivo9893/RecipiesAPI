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

        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            return await _context.Recipes
                .Include(r => r.Author)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Images)
                .ToListAsync();
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Author)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (recipe == null)
            {
                throw new KeyNotFoundException($"Recipe with Id {id} not found.");
            }
            return recipe;
        }

        public async Task<List<Recipe>> GetRecipesByAuthorIdAsync(int authorId)
        {
            return await _context.Recipes
                .Where(r => r.AuthorId == authorId)
                .Include(r => r.Author)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Images)
                .ToListAsync();
        }

        public async Task<Recipe> CreateRecipeAsync(CreateRecipeDTO dto)
        {
   
            var authorExists = await _context.Users.AnyAsync(u => u.Id == dto.AuthorId);
            if (!authorExists)
                throw new Exception($"Author with Id {dto.AuthorId} not found.");

            try
            {

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

                _context.Recipes.Add(recipe);

                await _context.SaveChangesAsync();

                await _categoryService.CreateRecipeCategoryAsync(dto.RecipeCategories, recipe.Id);

                await _ingredientService.CreateIngredientAsync(dto.RecipeIngredients, recipe.Id);

                await _imageService.CreateImageAsync(dto.Images, recipe.Id);

                return recipe;
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            throw new Exception("An error occurred while creating the recipe. Please try again later.");

        }
    }
}
