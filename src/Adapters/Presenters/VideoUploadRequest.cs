using System.Diagnostics.CodeAnalysis;

namespace Adapters.Presenters
{
    [ExcludeFromCodeCoverage]
    public record VideoUploadRequest(
        string FileName);
}
