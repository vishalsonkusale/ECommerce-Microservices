using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ApiGateway.Presentation.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AttachSignatureToRequest
    {
        private readonly RequestDelegate _next;

        public AttachSignatureToRequest(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.Headers["Api-Gateway"] = "signed";

            await _next(httpContext);
        }
    }
}
