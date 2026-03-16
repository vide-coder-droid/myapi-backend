using MyAPI.Repositories;
using MyAPI.Services;
using MyAPI.Services.Auth;
using MyAPI.Services.Profile;

namespace MyAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<JwtService>();
        services.AddScoped<CloudinaryService>();
        services.AddScoped<IUserRepository, DbUserRepository>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IProfileService, ProfileService>();

        return services;
    }
}