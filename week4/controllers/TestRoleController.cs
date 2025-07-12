using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    /// <summary>
    /// Test controller for role-based authorization testing
    /// This controller demonstrates POC and Admin role testing as requested
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestRoleController : ControllerBase
    {
        /// <summary>
        /// Test endpoint that requires POC role (should fail with Admin token)
        /// </summary>
        /// <returns>Success message if authorized</returns>
        [HttpGet("poc-only")]
        [Authorize(Roles = "POC")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<string> GetPocOnly()
        {
            return Ok("Access granted to POC role only");
        }

        /// <summary>
        /// Test endpoint that requires Admin or POC roles (should work with Admin token)
        /// </summary>
        /// <returns>Success message if authorized</returns>
        [HttpGet("admin-or-poc")]
        [Authorize(Roles = "Admin,POC")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<string> GetAdminOrPoc()
        {
            return Ok("Access granted to Admin or POC roles");
        }

        /// <summary>
        /// Test endpoint that requires Admin role only
        /// </summary>
        /// <returns>Success message if authorized</returns>
        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<string> GetAdminOnly()
        {
            return Ok("Access granted to Admin role only");
        }

        /// <summary>
        /// Test endpoint that requires any authenticated user
        /// </summary>
        /// <returns>Success message if authenticated</returns>
        [HttpGet("authenticated")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<string> GetAuthenticated()
        {
            var username = HttpContext.User.Identity?.Name ?? "Unknown";
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value ?? "Unknown";
            
            return Ok($"Access granted to authenticated user: {username} with role: {role}");
        }

        /// <summary>
        /// Test endpoint that allows anonymous access
        /// </summary>
        /// <returns>Success message for everyone</returns>
        [HttpGet("anonymous")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> GetAnonymous()
        {
            return Ok("Access granted to everyone (anonymous)");
        }
    }
}
