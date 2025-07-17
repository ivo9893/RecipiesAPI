using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models.DTO.Request
{
    public class CreateRecipeIngredientDTO
    {
        [Required(ErrorMessage = "Ingredient name is required.")]
        [MaxLength(200, ErrorMessage = "Ingredient name cannot exceed 200 characters.")]
        public string Name { get; set; }
        public int RecipeId { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0.01, 10000, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [MaxLength(50, ErrorMessage = "Unit cannot exceed 50 characters.")]
        public string Unit { get; set; }
    }
}
