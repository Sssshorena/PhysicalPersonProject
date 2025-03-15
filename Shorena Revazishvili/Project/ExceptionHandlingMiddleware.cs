using System.Net;
using System.Text.Json;

namespace Project
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly string _logDirectory;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            // ლოგების დირექტორიის შექმნა
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs/error-.txt");
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogExceptionToFile(ex);


                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An unexpected error occurred. Please try again later.",
                    Detail = ex.Message
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }

        private async Task LogExceptionToFile(Exception ex)
        {
            try
            {
                var logFileName = $"error_{DateTime.Now:yyyyMMdd}.log";
                var logFilePath = Path.Combine(_logDirectory, logFileName);

                var logEntry = new
                {
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Error = ex.Message,
                    Exception = new
                    {
                        Type = ex.GetType().FullName,
                        //StackTrace = ex.StackTrace,
                        Source = ex.Source,
                    }
                };

                var logMessage = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.AppendAllTextAsync(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception logEx)
            {
                _logger.LogError("Failed to write to log file: {ErrorMessage}", logEx.Message);
            }
        }
    }
}

