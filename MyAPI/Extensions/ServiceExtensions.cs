using MyAPI.Repositories;
using MyAPI.Services;
using MyAPI.Services.Auth;

namespace MyAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, DbUserRepository>();
        services.AddScoped<JwtService>();
        services.AddScoped<CloudinaryService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}