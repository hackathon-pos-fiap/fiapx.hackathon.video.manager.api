using Adapters.Gateways.MongoDbs.Entities;
using Adapters.Gateways.MongoDbs.Interfaces;
using Core.Entities;
using Core.Entities.Enums;
using Core.Gateways.Interfaces;

namespace Adapters.Gateways.MongoDbs.Converters
{
    public class VideoGatewayConverter : IVideoGateway
    {
        private readonly IVideoMongoDbGateway _videoMongoDbGateway;

        public VideoGatewayConverter(IVideoMongoDbGateway videoMongoDbGateway)
        {
            _videoMongoDbGateway = videoMongoDbGateway;
        }

        public async Task<IEnumerable<Video>> GetAllAsync(VideoStatus? status, string userId, int skip, int limit, CancellationToken cancellationToken)
        {
            var videoMongoDbList = await _videoMongoDbGateway.GetAllAsync(status, userId, skip, limit, cancellationToken);

            return VideoMongoDb.ToCore(videoMongoDbList);
        }

        public async Task<Video> GetByIdAsync(string id, string userId, CancellationToken cancellationToken)
        {
            var videoMongoDb = await _videoMongoDbGateway.GetByIdAsync(id, userId, cancellationToken);

            return VideoMongoDb.ToCore(videoMongoDb);
        }

        public async Task<Video> InsertAsync(Video video, CancellationToken cancellationToken)
        {
            var videoMongoDb = new VideoMongoDb(video);

            var insertedVideoMongoDb = await _videoMongoDbGateway.InsertAsync(videoMongoDb, cancellationToken);

            return VideoMongoDb.ToCore(insertedVideoMongoDb);
        }

        public async Task<Video> UpdateStatusAsync(string id, string userId, VideoStatus status, CancellationToken cancellationToken)
        {
            var videoMongoDb = await _videoMongoDbGateway.UpdateStatusAsync(id, userId, status, cancellationToken);
            
            return VideoMongoDb.ToCore(videoMongoDb);
        }
    }
}
