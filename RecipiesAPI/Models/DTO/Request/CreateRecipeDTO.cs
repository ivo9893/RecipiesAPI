using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models.DTO.Request
{
    public class CreateRecipeDTO : IValidatableObject
    {
        [Required(ErrorMessage = "Recipe name is required.")]
        [MaxLength(500, ErrorMessage = "Recipe name cannot exceed 500 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(5000, ErrorMessage = "Description cannot exceed 5000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Cook time is required.")]
        [Range(typeof(TimeSpan), "00:01:00", "24.00:00:00", ErrorMessage = "Cook time must be at least 1 minute.")]
        public TimeSpan CookTime { get; set; }

        [Required(ErrorMessage = "Prep time is required.")]
        [Range(typeof(TimeSpan), "00:01:00", "24.00:00:00", ErrorMessage = "Prep time must be at least 1 minute.")]
        public TimeSpan PrepTime { get; set; }



        [Range(1, 100, ErrorMessage = "Servings must be between 1 and 100.")]
        public int Servings { get; set; }


        [Required(ErrorMessage = "Author ID is required.")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Instructions are required.")]
        [MaxLength(10000, ErrorMessage = "Instructions cannot exceed 10,000 characters.")]
        public string Instructions { get; set; }

        public string RowID { get; set; }

        public List<CreateImageDTO> Images { get; set; } = new();

        public List<CreateRecipeCategoryDTO> RecipeCategories { get; set; } = new();

        public List<CreateRecipeIngredientDTO> RecipeIngredients { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RecipeIngredients == null || RecipeIngredients.Count == 0)
            {
                yield return new ValidationResult(
                    "At least one ingredient is required.",
                    new[] { nameof(RecipeIngredients) });
            }

            if (RecipeCategories == null || RecipeCategories.Count == 0)
            {
                yield return new ValidationResult(
                    "At least one category is required.",
                    new[] { nameof(RecipeCategories) });
            }
        }
    }
}
