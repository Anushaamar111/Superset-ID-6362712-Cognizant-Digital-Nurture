using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecureController : ControllerBase
    {
        [HttpGet("data")]
        [Authorize]
        public IActionResult GetSecureData()
        {
            return Ok("🔒 This is protected data.");
        }

        [HttpGet("public")]
        public IActionResult GetPublicData()
        {
            return Ok("🌐 This is public data - no authentication required.");
        }

        [HttpGet("user-info")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst("userId")?.Value;
            var username = User.FindFirst("username")?.Value;
            
            return Ok(new
            {
                Message = "User information retrieved successfully",
                UserId = userId,
                Username = username,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}
