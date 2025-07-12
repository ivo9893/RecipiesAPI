using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)] 
        public string Name { get; set; }

        [Required]
        public string Description { get; set; } 

        [Required]
        public TimeSpan CookTime { get; set; }
                                              

        [Required]
        public TimeSpan PrepTime { get; set; } 
                                               

        public int Servings { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; } 

        [ForeignKey("Author")]
        public int AuthorId { get; set; } 
        public User Author { get; set; } 

        [Required]
        public string Instructions { get; set; } 

        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<RecipeCategory> RecipeCategories { get; set; }
    }
}
