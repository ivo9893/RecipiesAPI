using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)] // Assuming a reasonable max length for names
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)] // Assuming a reasonable max length for email
        public string Email { get; set; }

        [Required]
        [MinLength(8)] // Assuming a minimum password length
        [MaxLength(255)] // Assuming a reasonable max length for hashed password
        public string Password { get; set; } // In a real application, this would be a hashed password

        // Navigation property for Recipes authored by this user
        public ICollection<Recipe> Recipes { get; set; }
    }
}
