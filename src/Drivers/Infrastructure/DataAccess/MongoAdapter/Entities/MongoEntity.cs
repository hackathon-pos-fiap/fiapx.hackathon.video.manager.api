using Infrastructure.DataAccess.MongoAdapter.Entities.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataAccess.MongoAdapter.Entities
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MongoEntity : IMongoEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public DateTime Created { get; set; }

        public MongoEntity()
        {
            Created = DateTime.UtcNow;
        }
    }
}
