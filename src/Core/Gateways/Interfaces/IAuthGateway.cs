namespace Core.Gateways.Interfaces
{
    public interface IAuthGateway
    {
        Task<string> GetUserIdFromTokenAsync(string token, string cpf, CancellationToken cancellationToken);
    }
}
