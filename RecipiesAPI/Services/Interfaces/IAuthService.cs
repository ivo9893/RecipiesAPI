using Google.Apis.Auth;
using RecipiesAPI.Models.DTO.Request;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponceDTO> LoginAsync(LoginDTO loginDto);
        Task<AuthResponceDTO> RefreshTokenAsync(string refreshToken);
        Task<AuthResponceDTO> VerifyGoogleTokenAsync(string idToken);
        Task<AuthResponceDTO> LoginFacebookAsync(LoginFacebookDTO userDTO);

    }
}
