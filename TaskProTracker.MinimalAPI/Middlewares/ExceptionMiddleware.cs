using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace TaskProTracker.MinimalAPI.Middlewares
{
    public static class ExceptionMiddleware
    {
        public static void UseGlobalExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandlerPathFeature != null)
                    {
                        var error = exceptionHandlerPathFeature.Error;

                        // Log the error using Serilog
                        Log.Error(error, "Unhandled exception caught in global middleware.");

                        await context.Response.WriteAsJsonAsync(new
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Message = "An unexpected error occurred. Please try again later."
                        });
                    }
                });
            });
        }
    }
}
