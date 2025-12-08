using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models.DTO.Request
{
    public class LoginFacebookDTO
    {
        [Required(ErrorMessage = "Access token is required.")]
        public string AccessToken { get; set; }
    }
}
