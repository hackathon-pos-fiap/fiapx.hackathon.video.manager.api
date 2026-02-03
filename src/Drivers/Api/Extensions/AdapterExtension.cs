using System.Diagnostics.CodeAnalysis;

namespace Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class AdapterExtension
    {
        public static IServiceCollection AddAdapter(this IServiceCollection services)
        {
            return services;
        }
    }
}
