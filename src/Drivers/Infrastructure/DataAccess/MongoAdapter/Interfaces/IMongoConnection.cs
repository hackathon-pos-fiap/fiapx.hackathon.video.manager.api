using MongoDB.Driver;

namespace Infrastructure.DataAccess.MongoAdapter.Interfaces
{
    public interface IMongoConnection
    {
        public string? AppName { get; }
        public string? ClusterName { get; }
        public IMongoClient Client { get; }
    }
}
