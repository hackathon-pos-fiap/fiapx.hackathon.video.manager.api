using System;
using System.Threading;
using Adapters.Gateways.BucketS3;
using Xunit;
using static UnitTests.VideoUseCaseTests;

namespace UnitTests
{
    public class BucketS3GatewayTests
    {
        [Fact]
        public void GenerateUploadUrl_ReturnsNonEmptyString()
        {
            var gateway = new FakeBucketGateway();

            var url = gateway.GenerateUploadUrl("file.mp4", CancellationToken.None);

            Assert.False(string.IsNullOrWhiteSpace(url));
            Assert.Contains("file.mp4", url);
        }

        [Fact]
        public void GenerateDownloadUrl_ReturnsNonEmptyString()
        {
            var gateway = new FakeBucketGateway();

            var url = gateway.GenerateDownloadUrl("file.mp4", CancellationToken.None);

            Assert.False(string.IsNullOrWhiteSpace(url));
            Assert.Contains("file.mp4", url);
        }
    }
}
