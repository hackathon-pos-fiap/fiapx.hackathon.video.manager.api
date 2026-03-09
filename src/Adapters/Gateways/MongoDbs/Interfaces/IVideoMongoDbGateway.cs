using Adapters.Gateways.MongoDbs.Entities;
using Core.Entities.Enums;

namespace Adapters.Gateways.MongoDbs.Interfaces
{
    public interface IVideoMongoDbGateway
    {
        Task<IEnumerable<VideoMongoDb>> GetAllAsync(VideoStatus? status, string userId, int skip, int limit, CancellationToken cancellationToken);
        Task<VideoMongoDb> GetByIdAsync(string id, string userId, CancellationToken cancellationToken);
        Task<VideoMongoDb> InsertAsync(VideoMongoDb video, CancellationToken cancellationToken);
        Task<VideoMongoDb> UpdateStatusAsync(string id, string userId, VideoStatus status, CancellationToken cancellationToken);
    }
}
