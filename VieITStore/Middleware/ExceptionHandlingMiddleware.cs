using System.Net;

namespace VieITStore.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi chưa được xử lý khi phục vụ {Method} {Path}", context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            if (ExpectsJson(context.Request))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Hệ thống đang gặp sự cố. Vui lòng thử lại sau."
                });
                return;
            }

            if (context.Request.Path.Equals("/Home/Error", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("Hệ thống đang gặp sự cố. Vui lòng thử lại sau.");
                return;
            }

            context.Response.Redirect("/Home/Error");
        }
    }

    private static bool ExpectsJson(HttpRequest request) =>
        request.Path.StartsWithSegments("/api")
        || string.Equals(request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
        || request.GetTypedHeaders().Accept?.Any(x =>
            string.Equals(x.MediaType.Value, "application/json", StringComparison.OrdinalIgnoreCase)) == true;
}
