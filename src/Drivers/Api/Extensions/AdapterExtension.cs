using Adapters.Controllers;
using Adapters.Controllers.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class AdapterExtension
    {
        public static IServiceCollection AddAdapter(this IServiceCollection services)
        {
            services.AddControllers();

            return services;
        }

        private static IServiceCollection AddControllers(this IServiceCollection services)
        {
            return services.AddScoped<IVideoController, VideoController>();
        }
    }
}
