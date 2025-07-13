using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models.DTO
{
    public class CreateRecipeDTO
    {
        [Required, MaxLength(500)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public TimeSpan CookTime { get; set; }

        [Required]
        public TimeSpan PrepTime { get; set; }

        public int Servings { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string Instructions { get; set; }

        public List<CreateImageDTO> Images { get; set; } = new();

        public List<CreateRecipeCategoryDTO> RecipeCategories { get; set; } = new();

        public List<CreateRecipeIngredientDTO> RecipeIngredients { get; set; } = new();
    }
}
