using Core.Entities.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Adapters.Presenters
{
    [ExcludeFromCodeCoverage]
    public record VideoResponse
    {
        public string? Id { get; set; }
        public string? FileName { get; set; }
        public string? UploadUrl { get; set; }
        public VideoStatus Status { get; set; }
    }
}
