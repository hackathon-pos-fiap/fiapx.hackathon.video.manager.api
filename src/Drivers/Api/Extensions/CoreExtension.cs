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
            services.AddUseCases();

            return services;
        }

        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            return services.AddSingleton<IVideoUseCase, VideoUseCase>();
        }
    }
}
