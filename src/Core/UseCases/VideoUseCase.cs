using Core.Entities;
using Core.Entities.Enums;
using Core.Gateways.Interfaces;
using Core.UseCases.Interfaces;

namespace Core.UseCases
{
    public class VideoUseCase : IVideoUseCase
    {
        private readonly IVideoGateway _videoGateway;

        public VideoUseCase(IVideoGateway videoGateway)
        {
            _videoGateway = videoGateway;
        }

        public async Task<IEnumerable<Video>> GetAllAsync(VideoStatus? status, int skip, int limit, CancellationToken cancellationToken)
        {
            return await _videoGateway.GetAllAsync(status, skip, limit, cancellationToken);
        }

        public async Task<Video> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _videoGateway.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Video> RequestUploadAsync(string fileName, CancellationToken cancellationToken)
        {
            return await _videoGateway.RequestUploadAsync(fileName, cancellationToken);
        }

        public async Task<Video> UpdateStatusAsync(string id, VideoStatus status, CancellationToken cancellationToken)
        {
            return await _videoGateway.UpdateStatusAsync(id, status, cancellationToken);
        }
    }
}
