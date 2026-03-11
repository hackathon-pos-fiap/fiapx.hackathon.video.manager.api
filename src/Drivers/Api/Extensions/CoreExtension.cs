using Core.Providers;
using Core.Providers.Interfaces;
using Core.UseCases;
using Core.UseCases.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class CoreExtension
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services
                .AddUseCases()
                .AddProviders();

            return services;
        }

        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            return services.AddScoped<IVideoUseCase, VideoUseCase>();
        }

        private static IServiceCollection AddProviders(this IServiceCollection services)
        {
            return services.AddScoped<IUserProvider, UserProvider>();
        }
    }
}
