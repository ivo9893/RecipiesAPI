using RecipiesAPI.Data;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
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

        public async Task<List<RecipeIngredient>> CreateIngredientAsync(List<CreateRecipeIngredientDTO> dto, int recipeId)
        {
            if (dto == null || !dto.Any())
            {
                throw new ArgumentException("Ingredient list cannot be null or empty.");
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

                if (ingredientDto.Unit == null)
                {
                    throw new ArgumentException("Unit cannot be empty.");
                }

                var ingredient = new RecipeIngredient
                {
                    RecipeId = recipeId,
                    Name = ingredientDto.Name,
                    Quantity = ingredientDto.Quantity,
                    UnitId = ingredientDto.Unit
                };
                ingredients.Add(ingredient);
            }
            _context.RecipeIngredients.AddRange(ingredients);
            _context.SaveChanges();

            return ingredients;
        }
    }
}
