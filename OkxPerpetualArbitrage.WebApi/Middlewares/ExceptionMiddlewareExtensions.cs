using Microsoft.AspNetCore.Diagnostics;
using OkxPerpetualArbitrage.Application.Exceptions;
using OkxPerpetualArbitrage.Application.Models;
using System.Net;

namespace OkxPerpetualArbitrage.WebApi.Middlewares
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, Microsoft.Extensions.Logging.ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    
                        ILogger l = app.ApplicationServices.GetRequiredService<ILogger<OkxPerpetualArbitrageCustomException>>();
                    if (exceptionHandlerPathFeature != null)
                    {
                        l.LogError($"Something went wrong: {exceptionHandlerPathFeature.Error}");
                        if (exceptionHandlerPathFeature?.Error is OkxPerpetualArbitrageCustomException)
                        {
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = exceptionHandlerPathFeature.Error.Message
                            }.ToString());
                        }
                        else
                        {
                            await context.Response.WriteAsync(new  ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = "Internal Server Error."
                            }.ToString());
                        }
                    }
                });
            });
        }
    }
}
