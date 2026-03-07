using Adapters.Controllers.Interfaces;
using Adapters.Presenters;
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

        public Task<IEnumerable<VideoResponse>> GetAllAsync(VideoFilter filter, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<VideoResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<VideoUploadResponse> RequestUploadAsync(VideoUploadRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<VideoResponse> UpdateStatusAsync(string id, VideoUpdateStatusRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
