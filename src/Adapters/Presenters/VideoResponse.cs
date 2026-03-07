using Core.Entities.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Adapters.Presenters
{
    [ExcludeFromCodeCoverage]
    public record VideoResponse(
        string? Id,
        string? FileName,
        string? UploadUrl,
        VideoStatus Status);
}
