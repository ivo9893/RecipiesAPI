using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)] 
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)] 
        public string Email { get; set; }

        [Required]
        [MinLength(8)] 
        [MaxLength(255)] 
        public string Password { get; set; } 

        public ICollection<Recipe> Recipes { get; set; }
    }
}
