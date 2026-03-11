using Core.Exceptions;
using Core.Gateways.Interfaces;
using Core.Providers.Interfaces;

namespace Api.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthGateway authGateway, IUserProvider userProvider)
        {
            string? authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            string? customerCpfHeader = context.Request.Headers["cpf"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedException("Missing or invalid token.");
            }

            if (string.IsNullOrEmpty(customerCpfHeader))
            {
                throw new UnauthorizedException("Missing cpf header value.");
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedException("Missing token.");
            }

            try
            {
                var userId = await authGateway.GetUserIdFromTokenAsync(token, customerCpfHeader, context.RequestAborted);
                userProvider.Id = userId;
                userProvider.Cpf = customerCpfHeader;
            }
            catch (Exception ex)
            {
                throw new UnauthorizedException($"Error while trying to authenticate. Details: {ex.Message}");
            }

            await _next(context);
        }
    }
}
