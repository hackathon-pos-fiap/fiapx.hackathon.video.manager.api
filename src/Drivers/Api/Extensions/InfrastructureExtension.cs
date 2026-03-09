using Infrastructure.DataAccess.MongoAdapter.Connections;
using Infrastructure.DataAccess.MongoAdapter.Factories;
using Infrastructure.DataAccess.MongoAdapter.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class InfrastructureExtension
    {
        private const string APP_NAME = "fiapx.hackathon.video.manager.api";
        private const string STRING_CONNECTION_MONGO = "StringConnectionMongo";

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services
                .AddDatabases()
                ;

            return services;
        }

        private static IServiceCollection AddDatabases(this IServiceCollection services)
        {
            var stringConnectionMongo = Environment.GetEnvironmentVariable(STRING_CONNECTION_MONGO);

            services.AddSingleton<IMongoConnection>(new MongoConnection("default", stringConnectionMongo!, APP_NAME));

            services.AddSingleton(DataContextFactory.Create);

            return services;
        }
    }
}