using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class Units
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property for related ingredients
        public ICollection<RecipeIngredient> Ingredients { get; set; }
    }
}
