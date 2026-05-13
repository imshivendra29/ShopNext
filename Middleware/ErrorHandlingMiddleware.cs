using ShopNext.Exceptions;
using System.Net;
using System.Text.Json;

namespace ShopNext.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); 
            }
            catch (AppException ex)
            {
                
                await WriteResponse(context, ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                
                await WriteResponse(context, 500, "Something went wrong");
            }
        }

        private async Task WriteResponse(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new { Message = message };
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}