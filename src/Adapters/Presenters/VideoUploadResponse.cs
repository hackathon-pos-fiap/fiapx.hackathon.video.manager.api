using System.Diagnostics.CodeAnalysis;

namespace Adapters.Presenters
{
    [ExcludeFromCodeCoverage]
    public record VideoUploadResponse(
        string VideoId,
        string UploadUrl);
}
