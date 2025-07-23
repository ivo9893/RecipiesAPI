using RecipiesAPI.Models;
using RecipiesAPI.Services.Interfaces;
using RecipiesAPI.Data;
using Microsoft.EntityFrameworkCore;
using RecipiesAPI.Models.DTO.Request;

namespace RecipiesAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(CreateUserDto userDto)
        {
            var existingUser = await _context.Users
                                             .FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"A user with the email '{userDto.Email}' already exists.");
            }


            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var newUser = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Password = hashedPassword 
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }
    }
}
