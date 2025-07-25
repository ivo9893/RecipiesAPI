using RecipiesAPI.Models;
using RecipiesAPI.Services.Interfaces;
using RecipiesAPI.Data;
using Microsoft.EntityFrameworkCore;
using RecipiesAPI.Models.DTO.Request;
using AutoMapper;
using RecipiesAPI.Models.DTO.Responce;

namespace RecipiesAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserDto userDto)
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

            var response = _mapper.Map<UserResponse>(newUser);

            return response;
        }
    }
}
