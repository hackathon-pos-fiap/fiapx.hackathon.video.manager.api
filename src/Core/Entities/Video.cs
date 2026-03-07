using Core.Entities.Enums;

namespace Core.Entities
{
    public class Video
    {
        public string Id { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string UploadUrl { get; set; } = string.Empty;
        public VideoStatus Status { get; set; }
    }
}
