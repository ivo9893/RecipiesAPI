using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)] // Assuming a reasonable max length for recipe name
        public string Name { get; set; }

        [Required]
        public string Description { get; set; } // Can be longer, so no MaxLength here unless specified

        [Required]
        public TimeSpan CookTime { get; set; } // Store as TimeSpan for duration
                                               // Alternatively, if stored as minutes/seconds in DB, use int CookTimeInMinutes;

        [Required]
        public TimeSpan PrepTime { get; set; } // Store as TimeSpan for duration
                                               // Alternatively, if stored as minutes/seconds in DB, use int PrepTimeInMinutes;

        public int Servings { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; } // Nullable if not always updated

        [ForeignKey("Author")]
        public int AuthorId { get; set; } // Foreign key to User
        public User Author { get; set; } // Navigation property

        [Required]
        public string Instructions { get; set; } // Can be very long

        // Navigation properties
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<RecipeCategory> RecipeCategories { get; set; }
    }
}
