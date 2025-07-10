using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)] // e.g., "Salad", "Meat"
        public string Name { get; set; }

        // Navigation property for Recipes in this category (many-to-many relationship)
        public ICollection<RecipeCategory> RecipeCategories { get; set; }
    }
}
