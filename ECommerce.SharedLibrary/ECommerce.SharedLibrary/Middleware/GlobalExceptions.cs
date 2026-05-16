//using ECommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ECommerce.SharedLibrary.Middleware
{
    public class GlobalExceptions(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "An unexpected error occurred.";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Internal Server Error";

            try
            {
                await next(context);

                // If Status Code is 429, handle it here as well
                if (context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
                {
                    title = "Too Many Requests";
                    message = "You have sent too many requests in a given amount of time. Please try again later.";
                    await ModifyHeaders(context, title, message, statusCode);
                }

                // Unauthorized access handling
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    title = "Unauthorized";
                    message = "Not authorized to access.";
                    await ModifyHeaders(context, title, message, statusCode);
                }

                // Forbidden access handling
                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    title = "Forbidden";
                    message = "You do not have permission to access this resource.";
                    await ModifyHeaders(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                //LogException.LogExceptions(ex);

                // If exception is timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Request Timeout";
                    message = "The request has timed out. Please try again later.";
                    statusCode = (int)HttpStatusCode.RequestTimeout;
                }

                await ModifyHeaders(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeaders(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Title = title,
                Status = statusCode
            }), CancellationToken.None);
            return;
        }
    }
}
