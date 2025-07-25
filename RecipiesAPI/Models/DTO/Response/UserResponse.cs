using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models.DTO.Responce
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

    }
}
