using Adapters.Gateways.ApiClients.DTOs;

namespace Adapters.Gateways.ApiClients.Interfaces
{
    public interface IAuthApiClientGateway
    {
        Task<AuthApiResponseDto> AuthenticateAsync(string token, string cpf, CancellationToken cancellationToken);
    }
}
