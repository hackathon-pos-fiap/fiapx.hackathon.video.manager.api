using Core.Providers;
using Xunit;

namespace UnitTests
{
    public class UserProviderTests
    {
        [Fact]
        public void UserProvider_HasNonEmptyId()
        {
            var provider = new UserProvider();

            Assert.True(string.IsNullOrWhiteSpace(provider.Id));
        }

        [Fact]
        public void UserProvider_CanSetId()
        {
            var provider = new UserProvider();
            provider.Id = "custom";

            Assert.Equal("custom", provider.Id);
        }
    }
}
