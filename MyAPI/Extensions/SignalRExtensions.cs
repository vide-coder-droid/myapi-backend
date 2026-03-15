using Microsoft.AspNetCore.SignalR;

namespace MyAPI.Extensions
{
    public static class SignalRExtensions
    {
        public static IServiceCollection AddSignalRConfig(this IServiceCollection services)
        {
            services.AddSignalR();

            return services;
        }
    }
}