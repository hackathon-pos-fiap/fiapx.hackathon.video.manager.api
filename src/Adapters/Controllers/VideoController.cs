using Adapters.Controllers.Interfaces;
using Adapters.Presenters;
using Core.Entities.Enums;
using Core.UseCases.Interfaces;

namespace Adapters.Controllers
{
    public class VideoController : IVideoController
    {
        private readonly IVideoUseCase _videoUseCase;

        public VideoController(IVideoUseCase videoUseCase)
        {
            _videoUseCase = videoUseCase;
        }

        public async Task<IEnumerable<VideoResponse>> GetAllAsync(VideoFilter filter, CancellationToken cancellationToken)
        {
            IEnumerable<VideoResponse> videos = await _videoUseCase.GetAllAsync(filter.Status, filter.Skip, filter.Limit, cancellationToken);

            return videos.Select(video => new VideoResponse(video.Id, video.FileName, video.UploadUrl, video.Status));
        }

        public Task<VideoResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var video = _videoUseCase.GetByIdAsync(id, cancellationToken);

            if (video is null)
            {
                throw new KeyNotFoundException($"Video with ID {id} not found.");
            }

            return new VideoResponse(video.Id, video.FileName, video.UploadUrl, video.Status);
        }

        public async Task<VideoUploadResponse> RequestUploadAsync(VideoUploadRequest request, CancellationToken cancellationToken)
        {
            var video = await _videoUseCase.RequestUploadAsync(request.FileName, cancellationToken);

            return new VideoUploadResponse(video.Id, video.UploadUrl);
        }

        public async Task<VideoResponse?> UpdateStatusAsync(string id, VideoUpdateStatusRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (request is null || request.Status == VideoStatus.None)
            {
                return null;
            }

            var video = await _videoUseCase.UpdateStatusAsync(id, request.Status, cancellationToken);

            return new VideoResponse(id, video.FileName, video.UploadUrl, video.Status);
        }
    }
}
