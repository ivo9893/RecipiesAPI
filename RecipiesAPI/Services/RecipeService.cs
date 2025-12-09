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
        private readonly ILogger<RecipeService> _logger;

        public RecipeService(AppDbContext context, IRecipeCategoryService categoryService, IRecipeIngredientService ingredientService, IImageService imageService, IMapper mapper, ILogger<RecipeService> logger)
        {
            _context = context;
            _categoryService = categoryService;
            _ingredientService = ingredientService;
            _imageService = imageService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<RecipeResponse>> GetAllRecipesAsync()
        {
            _logger.LogWarning("GetAllRecipesAsync called - consider using GetAllRecipesPagedAsync for better performance");

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

        public async Task<PagedResponse<RecipeResponse>> GetAllRecipesPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogDebug("Fetching recipes - Page: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);

            // Validate page parameters
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Max 100 items per page

            // Get total count
            var totalRecords = await _context.Recipes.CountAsync();

            // Fetch paginated data
            var recipes = await _context.Recipes
                .Include(r => r.Author)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Images)
                .OrderByDescending(r => r.CreatedAt) // Order by newest first
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = _mapper.Map<List<RecipeResponse>>(recipes);

            _logger.LogInformation("Fetched {Count} recipes (Page {PageNumber}/{TotalPages})",
                recipes.Count, pageNumber, Math.Ceiling(totalRecords / (double)pageSize));

            return new PagedResponse<RecipeResponse>(response, pageNumber, pageSize, totalRecords);
        }

        public async Task<RecipeResponse> GetRecipeByIdAsync(int id)
        {
            _logger.LogDebug("Fetching recipe with Id: {RecipeId}", id);

            var recipe = await _context.Recipes
                .Include(r => r.Author)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                _logger.LogWarning("Recipe with Id {RecipeId} not found", id);
                throw new KeyNotFoundException($"Recipe with Id {id} not found.");
            }

            var responce = _mapper.Map<RecipeResponse>(recipe);
            _logger.LogInformation("Successfully fetched recipe with Id: {RecipeId}", id);

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
            _logger.LogDebug("Fetching recipes for category Id: {CategoryId}", categoryId);

            var recipes = await _context.Recipes
                .Where(r => r.RecipeCategories.Any(rc => rc.CategoryId == categoryId))
                .Include(r => r.Author)
                .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Images)
                .ToListAsync();

            _logger.LogInformation("Found {Count} recipes for category Id: {CategoryId}", recipes.Count, categoryId);

            var response = _mapper.Map<List<RecipeResponse>>(recipes);

            return response;
        }

        public async Task<Recipe> CreateRecipeAsync(CreateRecipeDTO dto)
        {
            _logger.LogInformation("Creating recipe '{RecipeName}' for author Id: {AuthorId}", dto.Name, dto.AuthorId);

            var authorExists = await _context.Users.AnyAsync(u => u.Id == dto.AuthorId);

            if (!authorExists)
            {
                _logger.LogWarning("Failed to create recipe: Author with Id {AuthorId} not found", dto.AuthorId);
                throw new Exception($"Author with Id {dto.AuthorId} not found.");
            }

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
                    RowID = dto.RowID,
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

                _logger.LogInformation("Successfully created recipe '{RecipeName}' with Id: {RecipeId}", recipe.Name, recipe.Id);
                return recipe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating recipe '{RecipeName}' for author Id: {AuthorId}", dto.Name, dto.AuthorId);
                throw new Exception("An error occurred while creating the recipe. " + ex.Message);
            }
        }
    }
}
