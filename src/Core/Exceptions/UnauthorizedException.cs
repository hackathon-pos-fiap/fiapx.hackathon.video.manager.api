using System.Diagnostics.CodeAnalysis;

namespace Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
