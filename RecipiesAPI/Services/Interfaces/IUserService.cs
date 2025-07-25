using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Models.DTO.Responce;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(CreateUserDto userDto);
    }
}
