using Adapters.Gateways.ApiClients.Interfaces;
using Core.Gateways.Interfaces;

namespace Adapters.Gateways.ApiClients.Converters
{
    public class AuthApiGatewayConverter : IAuthGateway
    {
        private readonly IAuthApiClientGateway _authApiClientGateway;

        public AuthApiGatewayConverter(IAuthApiClientGateway authApiClientGateway)
        {
            _authApiClientGateway = authApiClientGateway;
        }

        public async Task<string> GetUserIdFromTokenAsync(string token, string cpf, CancellationToken cancellationToken)
        {
            var authResponse = await _authApiClientGateway.AuthenticateAsync(token, cpf, cancellationToken);
            return authResponse.CustomerIdentifier ?? string.Empty;
        }
    }
}
