using Adapters.Gateways.ApiClients.DTOs;
using Adapters.Gateways.ApiClients.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Adapters.Gateways.ApiClients
{
    public class AuthApiClientGateway : IAuthApiClientGateway
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public AuthApiClientGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthApiResponseDto> AuthenticateAsync(string token, string cpf, CancellationToken cancellationToken)
        {
            const string CUSTOMER_PATH = "/customer/{0}";

            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(CUSTOMER_PATH, cpf));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode is false)
            {
                throw new HttpRequestException($"Failed to authenticate for user {cpf}. Status code: {response.StatusCode}");
            }

            var authResponse = JsonSerializer.Deserialize<AuthApiResponseDto>(responseContent, _jsonSerializerOptions);

            return authResponse!;
        }
    }
}
