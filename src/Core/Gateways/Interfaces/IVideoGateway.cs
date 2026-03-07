using Core.Entities;
using Core.Entities.Enums;

namespace Core.Gateways.Interfaces
{
    public interface IVideoGateway
    {
        Task<IEnumerable<Video>> GetAllAsync(VideoStatus? status, int skip, int limit, CancellationToken cancellationToken);
        Task<Video> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<Video> RequestUploadAsync(string fileName, CancellationToken cancellationToken);
        Task<Video> UpdateStatusAsync(string id, VideoStatus status, CancellationToken cancellationToken);
    }
}
