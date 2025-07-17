using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using RecipiesAPI.Data;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Services
{
    public class RecipeCategoryService : IRecipeCategoryService
    {
        private readonly AppDbContext _context;

        public RecipeCategoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<RecipeCategory>> CreateRecipeCategoryAsync(List<CreateRecipeCategoryDTO> categories, int recipeId = -1)
        {

            if (recipeId == -1)
            {
                int RecipeId = categories.First().RecipeId;
                var recipeExists = await _context.Recipes.AnyAsync(r => r.Id == RecipeId);
                if (!recipeExists)
                {
                    throw new Exception($"Recipe with Id {RecipeId} not found.");
                }
            }

            var recipeCategories = new List<RecipeCategory>();
            foreach (var category in categories)
            {
                if (category.CategoryId == 0)
                {
                    throw new ArgumentException("CategoryId must be provided.");
                }


                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == category.CategoryId);
                if (!categoryExists)
                {
                    throw new Exception($"Category with Id {category.CategoryId} not found.");
                }

                var recipeCategory = new RecipeCategory
                {
                    CategoryId = category.CategoryId,
                    RecipeId = recipeId == -1 ? category.RecipeId : recipeId,
                    CreatedAt = DateTime.UtcNow
                };

                recipeCategories.Add(recipeCategory);
            }

         

            _context.RecipeCategories.AddRange(recipeCategories);
            await _context.SaveChangesAsync();

            return recipeCategories;
        }
    }
}
