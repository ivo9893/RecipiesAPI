using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipiesAPI.Data;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Models.DTO.Responce;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly AppDbContext _context;

        private readonly IRecipeCategoryService _categoryService;
        private readonly IRecipeIngredientService _ingredientService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public RecipeService(AppDbContext context, IRecipeCategoryService categoryService, IRecipeIngredientService ingredientService, IImageService imageService, IMapper mapper)
        {
            _context = context;
            _categoryService = categoryService;
            _ingredientService = ingredientService;
            _imageService = imageService;
            _mapper = mapper;
        }

        public async Task<List<RecipeResponse>> GetAllRecipesAsync()
        {
            var recipes = await _context.Recipes
                .Include(r => r.Author)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Images)
                .ToListAsync();

            var response = _mapper.Map<List<RecipeResponse>>(recipes);

            return response;

        }

        public async Task<RecipeResponse> GetRecipeByIdAsync(int id)
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

            var responce = _mapper.Map<RecipeResponse>(recipe);

            return responce;
        }

        public async Task<List<RecipeResponse>> GetRecipesByAuthorIdAsync(int authorId)
        {
            var recipes = await _context.Recipes
                .Where(r => r.AuthorId == authorId)
                .Include(r => r.Author)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Images)
                .ToListAsync();

            var response = _mapper.Map<List<RecipeResponse>>(recipes);

            return response;
        }

        public async Task<List<RecipeResponse>> GetRecipesByCategoryIdAsync(int categoryId)
        {
            var recipes = await _context.Recipes
            .Where(r => r.RecipeCategories.Any(rc => rc.CategoryId == categoryId)).ToListAsync();

            var response = _mapper.Map<List<RecipeResponse>>(recipes);

            return response;
        }

        public async Task<Recipe> CreateRecipeAsync(CreateRecipeDTO dto)
        {
            var authorExists = await _context.Users.AnyAsync(u => u.Id == dto.AuthorId);

            if (!authorExists)
                throw new Exception($"Author with Id {dto.AuthorId} not found.");
            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

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

                // Assuming these services also use the same DbContext instance (important for consistency)
                await _categoryService.CreateRecipeCategoryAsync(dto.RecipeCategories, recipe.Id);
                await _ingredientService.CreateIngredientAsync(dto.RecipeIngredients, recipe.Id);
                await _imageService.CreateImageAsync(dto.Images, recipe.Id);

                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();

                return recipe;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("An error occurred while creating the recipe. " + ex.Message);
            }
        }
    }
}
