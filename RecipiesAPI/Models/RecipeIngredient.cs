using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class RecipeIngredient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)] 
        public string Name { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } 

        public decimal Quantity { get; set; } 

        [MaxLength(50)] 
        public string Unit { get; set; }
    }
}
