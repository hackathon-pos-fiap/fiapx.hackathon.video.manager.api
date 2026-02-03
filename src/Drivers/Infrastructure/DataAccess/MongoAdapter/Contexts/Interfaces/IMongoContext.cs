using MongoDB.Driver;

namespace Infrastructure.DataAccess.MongoAdapter.Contexts.Interfaces
{
    public interface IMongoContext
    {
        public string ClusterName { get; }
        public IMongoDatabase Database { get; }

        IMongoCollection<TEntity> GetCollection<TEntity>();
    }
}
