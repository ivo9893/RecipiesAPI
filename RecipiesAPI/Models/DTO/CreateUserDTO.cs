using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models.DTO
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(255, ErrorMessage = "First name cannot exceed 255 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(255, ErrorMessage = "Last name cannot exceed 255 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters.")] 
        public string Password { get; set; }
    }
}
