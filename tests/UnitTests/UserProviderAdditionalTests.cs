using Core.Providers;
using Xunit;

namespace UnitTests
{
    public class UserProviderAdditionalTests
    {
        [Fact]
        public void Default_Id_IsNullAndCanBeSet()
        {
            var p = new UserProvider();
            Assert.Null(p.Id);

            p.Id = "u1";
            Assert.Equal("u1", p.Id);
        }

        [Fact]
        public void Cpf_Default_IsNullAndCanBeSet()
        {
            var p = new UserProvider();
            Assert.Null(p.Cpf);

            p.Cpf = "123";
            Assert.Equal("123", p.Cpf);
        }
    }
}
