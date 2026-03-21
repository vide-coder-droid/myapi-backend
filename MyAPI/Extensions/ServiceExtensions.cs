using MyAPI.Repositories;
using MyAPI.Services;
using MyAPI.Services.Auth;
using MyAPI.Services.Chat;
using MyAPI.Services.Profile;
using Microsoft.AspNetCore.Http;

namespace MyAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<JwtService>();
        services.AddScoped<CloudinaryService>();
        services.AddScoped<IUserRepository, DbUserRepository>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddSingleton<OtpService>();
        services.AddSingleton<EmailService>();
        services.AddSingleton<RedisService>();

        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IProfileService, ProfileService>();

        services.AddScoped<IConversationRepository, DbConversationRepository>();
        services.AddScoped<IChatService, ChatService>();

        return services;
    }
}