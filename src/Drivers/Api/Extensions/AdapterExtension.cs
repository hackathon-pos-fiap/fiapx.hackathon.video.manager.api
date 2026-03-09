using Adapters.Controllers;
using Adapters.Controllers.Interfaces;
using Adapters.Gateways.BucketS3;
using Adapters.Gateways.MongoDbs;
using Adapters.Gateways.MongoDbs.Converters;
using Adapters.Gateways.MongoDbs.Interfaces;
using Core.Gateways.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class AdapterExtension
    {
        public static IServiceCollection AddAdapters(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddGateways()
                .AddBuckets();

            return services;
        }

        private static IServiceCollection AddControllers(this IServiceCollection services)
        {
            return services.AddScoped<IVideoController, VideoController>();
        }

        private static IServiceCollection AddGateways(this IServiceCollection services)
        {
            return services
                .AddScoped<IVideoGateway, VideoGatewayConverter>()
                .AddScoped<IVideoMongoDbGateway, VideoMongoDbGateway>();
        }

        private static IServiceCollection AddBuckets(this IServiceCollection services)
        {
            return services
                .AddSingleton<IBucketGateway, BucketS3Gateway>();
        }
    }
}