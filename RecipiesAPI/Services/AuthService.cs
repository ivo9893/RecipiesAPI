using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipiesAPI.Data;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RecipiesAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponceDTO> LoginAsync(LoginDTO loginDto)
        {

            if(loginDto.Password == null) {
                return null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return null; // Invalid credentials
            }

            // Generate Access Token
            var accessToken = GenerateJwtToken(user);
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:TokenExpirationMinutes"]));

            // Generate and Save Refresh Token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]));

            var newRefreshToken = new Token
            {
                RefreshToken = refreshToken,
                ExpiryDate = refreshTokenExpiry,
                UserId = user.Id
            };

            _context.Token.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponceDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessTokenExpiry,
                UserId = user.Id,
                Email = user.Email
            };
        }

        // --- Refresh Token Method ---
        public async Task<AuthResponceDTO> RefreshTokenAsync(string refreshToken)
        {
            var existingRefreshToken = await _context.Token
                                                    .Include(rt => rt.User) // Eager load user
                                                    .SingleOrDefaultAsync(rt => rt.RefreshToken == refreshToken);

            if (existingRefreshToken == null || existingRefreshToken.IsRevoked || existingRefreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                return null; // Invalid, revoked, or expired refresh token
            }

            // Revoke the old refresh token (one-time use pattern)
            existingRefreshToken.RevokedAt = DateTime.UtcNow;
            _context.Token.Update(existingRefreshToken);

            // Generate new Access Token
            var newAccessToken = GenerateJwtToken(existingRefreshToken.User);
            var newAccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:TokenExpirationMinutes"]));

            // Generate new Refresh Token
            var newRefreshTokenValue = GenerateRefreshToken();
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]));

            var newRefreshToken = new Token
            {
                RefreshToken = newRefreshTokenValue,
                ExpiryDate = newRefreshTokenExpiry,
                UserId = existingRefreshToken.User.Id
            };

            _context.Token.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponceDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue,
                AccessTokenExpiry = newAccessTokenExpiry,
                UserId = existingRefreshToken.User.Id,
                Email = existingRefreshToken.User.Email
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes = int.Parse(jwtSettings["TokenExpirationMinutes"]);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes), 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32]; 
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<AuthResponceDTO> VerifyGoogleTokenAsync(string idToken) {
            try {
                var settings = new GoogleJsonWebSignature.ValidationSettings() {
                    Audience = new[] { _configuration["GoogleWebClientID"] } 
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);
                if (user == null) {
                    user = new User {
                        Email = payload.Email,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        GoogleId = payload.Subject,
                        Password = null // or random placeholder
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Generate Access Token
                var accessToken = GenerateJwtToken(user);
                var accessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["JwtSettings:TokenExpirationMinutes"]));

                // Generate and Save Refresh Token
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(
                    int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]));

                var newRefreshToken = new Token {
                    RefreshToken = refreshToken,
                    ExpiryDate = refreshTokenExpiry,
                    UserId = user.Id
                };

                _context.Token.Add(newRefreshToken);
                await _context.SaveChangesAsync();

                return new AuthResponceDTO {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = accessTokenExpiry,
                    UserId = user.Id,
                    Email = user.Email
                };

            } catch (Exception ex) {
                throw new UnauthorizedAccessException("Invalid Google ID token.", ex);
            }
        }

        public async Task<AuthResponceDTO> LoginFacebookAsync(LoginFacebookDTO userDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDTO.email);
            if (user == null)
            {
                user = new User
                {
                    Email = userDTO.email,
                    FirstName = userDTO.firstName,
                    LastName = userDTO.lastName,
                    Password = null 
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Generate Access Token
            var accessToken = GenerateJwtToken(user);
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:TokenExpirationMinutes"]));

            // Generate and Save Refresh Token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]));

            var newRefreshToken = new Token
            {
                RefreshToken = refreshToken,
                ExpiryDate = refreshTokenExpiry,
                UserId = user.Id
            };

            _context.Token.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponceDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessTokenExpiry,
                UserId = user.Id,
                Email = user.Email
            };


        }
    }
}

