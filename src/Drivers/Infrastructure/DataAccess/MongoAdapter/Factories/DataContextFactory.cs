using Infrastructure.DataAccess.MongoAdapter.Contexts;
using Infrastructure.DataAccess.MongoAdapter.Contexts.Interfaces;
using Infrastructure.DataAccess.MongoAdapter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataAccess.MongoAdapter.Factories
{
    [ExcludeFromCodeCoverage]
    public static class DataContextFactory
    {
        public static IMongoContext Create(IServiceProvider serviceProvider)
        {
            var mongoConnections = serviceProvider.GetServices<IMongoConnection>();

            var mongoConnection = mongoConnections.Where(w => w.ClusterName == "default").FirstOrDefault();

            var mongoDatabase = mongoConnection!.Client.GetDatabase("fiap_hackathon");

            return new MongoContext("default", mongoDatabase);
        }
    }
}
