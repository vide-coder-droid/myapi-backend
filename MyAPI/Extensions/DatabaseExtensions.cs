using Microsoft.EntityFrameworkCore;
using MyAPI.Data;

namespace MyAPI.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                config.GetConnectionString("DefaultConnection"),
                o => o.EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(10),
                    null));
        });

        return services;
    }
}