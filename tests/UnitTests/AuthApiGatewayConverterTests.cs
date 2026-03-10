using System.Threading;
using System.Threading.Tasks;
using Adapters.Gateways.ApiClients.Converters;
using Adapters.Gateways.ApiClients.DTOs;
using Adapters.Gateways.ApiClients.Interfaces;
using Moq;
using Xunit;

namespace UnitTests
{
    public class AuthApiGatewayConverterTests
    {
        [Fact]
        public async Task GetUserIdFromTokenAsync_ReturnsCustomerIdentifier_WhenPresent()
        {
            var mockClient = new Mock<IAuthApiClientGateway>();
            mockClient.Setup(m => m.AuthenticateAsync("token1", "cpf1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AuthApiResponseDto { CustomerIdentifier = "user-123" });

            var converter = new AuthApiGatewayConverter(mockClient.Object);

            var result = await converter.GetUserIdFromTokenAsync("token1", "cpf1", CancellationToken.None);

            Assert.Equal("user-123", result);
            mockClient.Verify(m => m.AuthenticateAsync("token1", "cpf1", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUserIdFromTokenAsync_ReturnsEmptyString_WhenCustomerIdentifierIsNull()
        {
            var mockClient = new Mock<IAuthApiClientGateway>();
            mockClient.Setup(m => m.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AuthApiResponseDto { CustomerIdentifier = null });

            var converter = new AuthApiGatewayConverter(mockClient.Object);

            var result = await converter.GetUserIdFromTokenAsync("token2", "cpf2", CancellationToken.None);

            Assert.Equal(string.Empty, result);
            mockClient.Verify(m => m.AuthenticateAsync("token2", "cpf2", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
