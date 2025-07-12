using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFirstApi.Controllers
{
    /// <summary>
    /// Test Authentication controller for JWT token generation with 2-minute expiration
    /// This controller is specifically for testing JWT expiration as requested in the requirements
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // In-memory user store (In production, use a database)
        private static readonly List<User> _users = new List<User>
        {
            new User 
            { 
                Id = 1, 
                Username = "admin", 
                Password = "admin123", 
                Role = "Admin", 
                Email = "admin@company.com", 
                FullName = "System Administrator" 
            },
            new User 
            { 
                Id = 2, 
                Username = "manager", 
                Password = "manager123", 
                Role = "Manager", 
                Email = "manager@company.com", 
                FullName = "HR Manager" 
            },
            new User 
            { 
                Id = 3, 
                Username = "user", 
                Password = "user123", 
                Role = "User", 
                Email = "user@company.com", 
                FullName = "Regular User" 
            }
        };

        /// <summary>
        /// Constructor for TestAuthController
        /// </summary>
        /// <param name="configuration">Configuration service</param>
        public TestAuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Login endpoint to generate JWT token with 2-minute expiration for testing
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>JWT token with 2-minute expiration</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                // Validate user credentials
                var user = _users.FirstOrDefault(u => 
                    u.Username == loginRequest.Username && 
                    u.Password == loginRequest.Password);

                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Generate JWT token with 2-minute expiration
                var token = GenerateJSONWebToken(user.Id, user.Role);
                var expiration = DateTime.UtcNow.AddMinutes(2); // 2 minutes for testing

                var response = new LoginResponse
                {
                    Token = token,
                    Expiration = expiration,
                    User = new User
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Role = user.Role,
                        Email = user.Email,
                        FullName = user.FullName
                        // Don't return password in response
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate JWT token with 2-minute expiration for testing
        /// This method follows the exact format requested in the requirements
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userRole">User role</param>
        /// <returns>JWT token string</returns>
        private string GenerateJSONWebToken(int userId, string userRole)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _users.FirstOrDefault(u => u.Id == userId)?.Username ?? "Unknown"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, _users.FirstOrDefault(u => u.Id == userId)?.Username ?? "Unknown"),
                new Claim(ClaimTypes.Role, userRole),
                new Claim("UserId", userId.ToString())
            };

            var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(2), // 2 minutes for testing expiration
                        signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
