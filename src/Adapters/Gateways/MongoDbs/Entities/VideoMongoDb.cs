using Core.Entities;
using Core.Entities.Enums;
using Infrastructure.DataAccess.MongoAdapter.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Adapters.Gateways.MongoDbs.Entities
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("video")]
    [ExcludeFromCodeCoverage]
    public class VideoMongoDb : MongoEntity
    {
        public VideoMongoDb(Video video)
        {
            UserId = video.UserId;
            FileName = video.FileName;
            Status = video.Status;
        }

        public string UserId { get; set; }
        public string FileName { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public VideoStatus Status { get; set; }

        public static IEnumerable<Video> ToCore(IEnumerable<VideoMongoDb> videoMongoDbList)
        {
            return videoMongoDbList.Select(ToCore);
        }

        public static Video ToCore(VideoMongoDb videoMongoDb)
        {
            return new Video
            {
                Id = videoMongoDb.Id,
                UserId = videoMongoDb.UserId,
                FileName = videoMongoDb.FileName,
                Status = videoMongoDb.Status
            };
        }

        public Video ToCore()
        {
            return ToCore(this);
        }
    }
}
