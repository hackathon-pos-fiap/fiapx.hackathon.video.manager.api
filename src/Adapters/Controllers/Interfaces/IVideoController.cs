using Adapters.Presenters;

namespace Adapters.Controllers.Interfaces
{
    public interface IVideoController
    {
        Task<VideoUploadResponse> RequestUploadAsync(VideoUploadRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<VideoResponse>> GetAllAsync(VideoFilter filter, CancellationToken cancellationToken);
        Task<VideoResponse> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<VideoResponse?> UpdateStatusAsync(string id, VideoUpdateStatusRequest request, CancellationToken cancellationToken);
    }
}
