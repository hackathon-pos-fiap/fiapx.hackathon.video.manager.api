using Adapters.Controllers;
using Adapters.Controllers.Interfaces;
using Adapters.Gateways.ApiClients;
using Adapters.Gateways.ApiClients.Converters;
using Adapters.Gateways.ApiClients.Interfaces;
using Adapters.Gateways.BucketS3;
using Adapters.Gateways.MongoDbs;
using Adapters.Gateways.MongoDbs.Converters;
using Adapters.Gateways.MongoDbs.Interfaces;
using Core.Gateways.Interfaces;
using Polly;
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
            services.AddApiClients();

            return services
                .AddScoped<IAuthGateway, AuthApiGatewayConverter>()
                .AddScoped<IVideoGateway, VideoGatewayConverter>()
                .AddScoped<IVideoMongoDbGateway, VideoMongoDbGateway>();
        }

        private static void AddApiClients(this IServiceCollection services)
        {
            var authApiUrl = Environment.GetEnvironmentVariable("AUTH_API_URL");

            services.AddHttpClient<IAuthApiClientGateway, AuthApiClientGateway>(client =>
            {
                client.BaseAddress = new Uri(authApiUrl!);
            })
            .AddTransientHttpErrorPolicy(policyBuilder =>
            {
                return policyBuilder.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(0.4 * Math.Pow(2, attempt)));
            });
        }

        private static IServiceCollection AddBuckets(this IServiceCollection services)
        {
            return services
                .AddSingleton<IBucketGateway, BucketS3Gateway>();
        }
    }
}