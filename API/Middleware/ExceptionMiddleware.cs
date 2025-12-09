using System;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next,
ILogger<ExceptionMiddleware> logger, 
IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);//προχωράω στην επομενη middleware λειτουργία αφου ολοκληρωθεί αυτή γιαυτο έχω το await
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"{message}", ex.Message);

            //returning response to client type json
            context.Response.ContentType="application/json";

            //setting status code =500
            context.Response.StatusCode= (int)HttpStatusCode.InternalServerError;//=500 error server

            //different response for development and production
            var response = env.IsDevelopment()
            ? new ApiException(context.Response.StatusCode,ex.Message,ex.StackTrace)
            : new ApiException(context.Response.StatusCode,ex.Message,"Internal Server Error");

            //serialize response to json
            // απο PascalCase που έχω στην C#  -> σε camelCase που θέλει το javascript
            var option = new JsonSerializerOptions
            {
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase
            };
            // convert response to json string
            var json=JsonSerializer.Serialize(response,option);

            //write to response
            await context.Response.WriteAsync(json);
        }
    }

}
