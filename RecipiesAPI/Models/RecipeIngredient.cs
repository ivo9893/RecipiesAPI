using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class RecipeIngredient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)] // e.g., "Salt", "Milk"
        public string Name { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } // Navigation property

        public decimal Quantity { get; set; } // Use decimal for precise quantities
                                              // Alternatively, if quantities are always whole numbers, use int

        [MaxLength(50)] // e.g., "g", "ml", "tsp", "cups"
        public string Unit { get; set; }
    }
}
