using Microsoft.AspNetCore.Http;
using System.Net;

namespace ECommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next) 
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var signedHeaders = context.Response.Headers["Api-Gateway"];

            // If request is not from API-Gateway, then return Service Unavailable
            if (signedHeaders.FirstOrDefault() is null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await context.Response.WriteAsync("Service Unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
