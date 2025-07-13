using RecipiesAPI.Data;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Services
{
    public class RecipeIngredientService : IRecipeIngredientService
    {
        private readonly AppDbContext _context;

        public RecipeIngredientService(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<RecipeIngredient>> CreateIngredientAsync(List<CreateRecipeIngredientDTO> dto, int recipeId = -1)
        {
            if (dto == null || !dto.Any())
            {
                throw new ArgumentException("Ingredient list cannot be null or empty.");
            }

            if (recipeId == -1)
            {
                int recipe = dto.First().RecipeId;
                var recipeExists = _context.Recipes.Any(r => r.Id == recipe);
                if (!recipeExists)
                {
                    throw new Exception($"Recipe with Id {recipe} not found.");
                }
            }

            var ingredients = new List<RecipeIngredient>();
            foreach (var ingredientDto in dto)
            {
                if (string.IsNullOrEmpty(ingredientDto.Name))
                {
                    throw new ArgumentException("Ingredient name must be provided.");
                }

                if (ingredientDto.Quantity <= 0)
                {
                    throw new ArgumentException("Ingredient quantity must be greater than zero.");
                }

                if (string.IsNullOrEmpty(ingredientDto.Unit))
                {
                    throw new ArgumentException("Unit cannot be empty.");
                }

                var ingredient = new RecipeIngredient
                {
                    RecipeId = recipeId == -1 ? ingredientDto.RecipeId : recipeId,
                    Name = ingredientDto.Name,
                    Quantity = ingredientDto.Quantity,
                    Unit = ingredientDto.Unit
                };
                ingredients.Add(ingredient);
            }
            _context.RecipeIngredients.AddRange(ingredients);
            _context.SaveChanges();
            return Task.FromResult(ingredients);
        }
    }
}
