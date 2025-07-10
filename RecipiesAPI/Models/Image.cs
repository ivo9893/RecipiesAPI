using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } // Navigation property

        [Required]
        [Url]
        [MaxLength(2048)] // Standard max length for URLs
        public string Url { get; set; } // URL to the image
    }
}
