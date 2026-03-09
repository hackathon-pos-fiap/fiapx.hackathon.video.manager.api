using Core.Entities;
using Core.Entities.Enums;

namespace Core.Gateways.Interfaces
{
    public interface IVideoGateway
    {
        Task<IEnumerable<Video>> GetAllAsync(VideoStatus? status, string userId, int skip, int limit, CancellationToken cancellationToken);
        Task<Video> GetByIdAsync(string id, string userId, CancellationToken cancellationToken);
        Task<Video> InsertAsync(Video video, CancellationToken cancellationToken);
        Task<Video> UpdateStatusAsync(string id, string userId, VideoStatus status, CancellationToken cancellationToken);
    }
}
