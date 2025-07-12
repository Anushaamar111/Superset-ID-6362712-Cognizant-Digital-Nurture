using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Models
{
    /// <summary>
    /// User model for authentication
    /// </summary>
    public class User
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username for authentication
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for authentication
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// User role for authorization
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// User email
        /// </summary>
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User full name
        /// </summary>
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Login request model
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Username for login
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for login
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Login response model
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// User information
        /// </summary>
        public User User { get; set; } = new User();
    }
}
