using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class RecipeCategory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } 

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } 

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
