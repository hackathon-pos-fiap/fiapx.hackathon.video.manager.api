using Core.Exceptions;

namespace Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedException ex)
            {
                await HandleExceptionAsync(context, ex, StatusCodes.Status401Unauthorized);
            }
            catch (DuplicateItemException ex)
            {
                await HandleExceptionAsync(context, ex, StatusCodes.Status409Conflict);
            }
            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(context, ex, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                type = ex.GetType().Name
            });
        }
    }
}
