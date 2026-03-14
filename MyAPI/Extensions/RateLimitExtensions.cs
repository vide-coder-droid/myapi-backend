using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace MyAPI.Extensions;

public static class RateLimitExtensions
{
    public static IServiceCollection AddRateLimiterConfig(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("api", opt =>
            {
                opt.PermitLimit = 100;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueLimit = 0;
            });
        });

        return services;
    }
}