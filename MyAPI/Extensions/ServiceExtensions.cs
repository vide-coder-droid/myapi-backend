using MyAPI.Repositories;
using MyAPI.Services;

namespace MyAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, FakeUserRepository>();
        services.AddSingleton<JwtService>();

        return services;
    }
}