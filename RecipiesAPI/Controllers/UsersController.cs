using Microsoft.AspNetCore.Mvc;
using RecipiesAPI.Models;
using RecipiesAPI.Services.Interfaces;
using RecipiesAPI.Models.DTO.Request;

namespace RecipiesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // e.g., /api/users
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")] 
        [ProducesResponseType(typeof(User), 201)] // 201 Created
        [ProducesResponseType(400)] // Bad Request for validation errors
        [ProducesResponseType(409)] // Conflict if email already exists
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Register([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns validation errors
            }

            try
            {
                var createdUser = await _userService.CreateUserAsync(userDto);

                createdUser.Password = null;

                return CreatedAtAction(nameof(Register), new { id = createdUser.Id }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }
    }
}
