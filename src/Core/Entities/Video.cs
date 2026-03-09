using Core.Entities.Enums;

namespace Core.Entities
{
    public class Video
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? UploadUrl { get; set; }
        public string? DownloadUrl { get; set; }
        public VideoStatus Status { get; set; }
    }
}
