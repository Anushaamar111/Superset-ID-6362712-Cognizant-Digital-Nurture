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
    /// Authentication controller for JWT token generation
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
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
        /// Constructor for AuthController
        /// </summary>
        /// <param name="configuration">Configuration service</param>
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Login endpoint to generate JWT token
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>JWT token and user information</returns>
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

                // Generate JWT token
                var token = GenerateJSONWebToken(user);
                var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));

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
        /// Get current user information (requires authentication)
        /// </summary>
        /// <returns>Current user information</returns>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(401)]
        public ActionResult<User> GetCurrentUser()
        {
            try
            {
                var username = HttpContext.User.Identity?.Name;
                var user = _users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    Email = user.Email,
                    FullName = user.FullName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        /// <returns>List of all users</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        [ProducesResponseType(typeof(List<User>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                var users = _users.Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role,
                    Email = u.Email,
                    FullName = u.FullName
                }).ToList();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Refresh JWT token (requires valid token)
        /// </summary>
        /// <returns>New JWT token</returns>
        [Authorize]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(401)]
        public ActionResult<LoginResponse> RefreshToken()
        {
            try
            {
                var username = HttpContext.User.Identity?.Name;
                var user = _users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    return Unauthorized("Invalid user");
                }

                var token = GenerateJSONWebToken(user);
                var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));

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
        /// Generate JWT token for authenticated user
        /// </summary>
        /// <param name="user">User information</param>
        /// <returns>JWT token string</returns>
        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
