using RecipiesAPI.Models.DTO;
using RecipiesAPI.Models;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(CreateUserDto userDto);
    }
}
