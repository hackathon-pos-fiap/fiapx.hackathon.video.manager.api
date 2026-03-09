using System.Diagnostics.CodeAnalysis;

namespace Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException()
        {
        }

        public DuplicateItemException(string? message) : base(message)
        {
        }
    }
}
