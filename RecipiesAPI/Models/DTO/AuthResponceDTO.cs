namespace RecipiesAPI.Models.DTO
{
    public class AuthResponceDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        // Optional: User info
        public int UserId { get; set; }
        public string Email { get; set; }
    }
}
