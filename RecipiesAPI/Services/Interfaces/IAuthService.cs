using RecipiesAPI.Models.DTO.Request;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponceDTO> LoginAsync(LoginDTO loginDto);
        Task<AuthResponceDTO> RefreshTokenAsync(string refreshToken); 
    }
}
