using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyFirstApi.Filters
{
    public class CustomAuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            
            // Check if Authorization header exists
            if (!request.Headers.ContainsKey("Authorization"))
            {
                context.Result = new BadRequestObjectResult(new { message = "Invalid request - No Auth token" });
                return;
            }

            // Get the Authorization header value
            var authHeader = request.Headers["Authorization"].FirstOrDefault();
            
            // Check if the header contains "Bearer"
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new BadRequestObjectResult(new { message = "Invalid request - Token present but Bearer unavailable" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
