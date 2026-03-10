using Core.Entities.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Adapters.Presenters
{
    [ExcludeFromCodeCoverage]
    public record VideoResponse(
        string? Id,
        string? UserId,
        string? FileName,
        string? DownloadUrl,
        VideoStatus Status);
}
