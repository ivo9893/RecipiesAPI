using Microsoft.AspNetCore.Mvc;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Services.Interfaces;
using Google.Apis.Auth;
namespace RecipiesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT access token and a refresh token.
        /// </summary>
        /// <param name="loginDto">User credentials (email and password).</param>
        /// <returns>A JWT access token and refresh token if successful, otherwise 401 Unauthorized.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponceDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authResponse = await _authService.LoginAsync(loginDto);

            if (authResponse == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(authResponse);
        }

        /// <summary>
        /// Refreshes the access token using a valid refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>New access token and refresh token.</returns>
        [HttpPost("refresh-token")] // e.g., /api/auth/refresh-token
        [ProducesResponseType(typeof(AuthResponceDTO), 200)]
        [ProducesResponseType(400)] // For missing/invalid refresh token in request
        [ProducesResponseType(401)] // For invalid/expired/revoked refresh token
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken) // Expecting raw string in body
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new { message = "Refresh token is required." });
            }

            var authResponse = await _authService.RefreshTokenAsync(refreshToken);

            if (authResponse == null)
            {
                return Unauthorized(new { message = "Invalid, expired, or revoked refresh token." });
            }

            return Ok(authResponse);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleSignIn([FromBody] string token) {
            try {

                var authResponse = await _authService.VerifyGoogleTokenAsync(token);

                if (authResponse == null) {
                    return Unauthorized(new { message = "Authentication failed." });
                }

                return Ok(authResponse);

            } catch (UnauthorizedAccessException) {
                return Unauthorized(new { message = "Invalid Google ID token." });
            }
        }

        [HttpPost("facebook")]
        public async Task<IActionResult> FacebookSignIn([FromBody] LoginFacebookDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var authResponse = await _authService.LoginFacebookAsync(user);

                if (authResponse == null)
                {
                    return Unauthorized(new { message = "Authentication failed." });
                }

                return Ok(authResponse);

            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid Facebook access token." });
            }
        }
    }
}
