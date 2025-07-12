using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace MyFirstApi.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(IWebHostEnvironment environment, ILogger<CustomExceptionFilter> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            
            // Log the exception details to a file
            LogExceptionToFile(exception, context.HttpContext.Request.Path);

            // Create a custom error response
            var errorResponse = new
            {
                Error = "Internal Server Error",
                Message = exception.Message,
                StatusCode = 500,
                Path = context.HttpContext.Request.Path.Value,
                Timestamp = DateTime.UtcNow
            };

            // Set the result
            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = 500
            };

            // Mark the exception as handled
            context.ExceptionHandled = true;
        }

        private void LogExceptionToFile(Exception exception, string path)
        {
            try
            {
                var logDirectory = Path.Combine(_environment.ContentRootPath, "Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var logFileName = $"exceptions_{DateTime.Now:yyyyMMdd}.log";
                var logFilePath = Path.Combine(logDirectory, logFileName);

                var logEntry = new
                {
                    Timestamp = DateTime.UtcNow,
                    Path = path,
                    ExceptionType = exception.GetType().Name,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    InnerException = exception.InnerException?.Message
                };

                var logText = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });
                var logLine = $"{Environment.NewLine}{'='}{new string('=', 50)}{Environment.NewLine}{logText}{Environment.NewLine}";

                File.AppendAllText(logFilePath, logLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log exception to file");
            }
        }
    }
}
