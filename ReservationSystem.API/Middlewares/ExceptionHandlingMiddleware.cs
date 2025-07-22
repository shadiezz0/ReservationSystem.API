using ReservationSystem.Domain.Shared;
using System.Net;
using System.Text;
using System.Text.Json;
using static ReservationSystem.Domain.Constants.Enums;

namespace ReservationSystem.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Read request body for logging
                context.Request.EnableBuffering();
                var bodyStream = new StreamReader(context.Request.Body);
                var bodyText = await bodyStream.ReadToEndAsync();
                context.Request.Body.Position = 0;

                // Store the body text to context for later use if needed
                context.Items["RequestBody"] = bodyText;

                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var request = context.Request;

            var bodyText = context.Items["RequestBody"] as string ?? "";

            var logText = new StringBuilder();
            logText.AppendLine("============================================================");
            logText.AppendLine($"Timestamp      : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            logText.AppendLine($"Request Path   : {request.Path}");
            logText.AppendLine($"Method         : {request.Method}");
            logText.AppendLine($"Query String   : {request.QueryString}");
            logText.AppendLine($"Body           : {bodyText}");
            logText.AppendLine();
            logText.AppendLine($"Exception Type : {ex.GetType().Name}");
            logText.AppendLine($"Message        : {ex.Message}");
            logText.AppendLine("Stack Trace    :");
            logText.AppendLine(ex.StackTrace);
            logText.AppendLine("============================================================");

            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "errors");
            Directory.CreateDirectory(logDirectory); // create if doesn't exist

            var logFilePath = Path.Combine(logDirectory, $"Exception-log-{DateTime.Now:yyyy-MM-dd}.txt");

            await File.AppendAllTextAsync(logFilePath, logText.ToString());

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ResponseResult
            {
                Result = Result.Failed,
                Alart = new Alart
                {
                    AlartType = AlartType.error,
                    type = AlartShow.note,
                    MessageAr = "حدث خطأ غير متوقع في الخادم.",
                    MessageEn = "An unexpected error occurred on the server."
                }
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
