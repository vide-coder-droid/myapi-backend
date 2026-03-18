using MyAPI.Data;
using MyAPI.Extensions;
using Scalar.AspNetCore;
using Serilog;
using MyAPI.Hubs;
using MyAPI.Middleware;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Port
builder.ConfigurePort();

// Logging
builder.ConfigureLogging();

// Services
builder.Services.AddControllers();
builder.Services.AddApplicationServices();

// Infrastructure / Middleware Config
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocs();
builder.Services.AddRateLimiterConfig();
builder.Services.AddCorsPolicy();
builder.Services.AddHealthChecks();
builder.Services.AddSignalRConfig();

// File upload limit
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024;
});


var app = builder.Build();
app.ApplyMigration();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocs();

    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
    });
}

app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRateLimiter();

app.UseAuthentication();
app.UseMiddleware<ActiveUserMiddleware>();
app.UseAuthorization();

// Routes
app.MapControllers().RequireRateLimiting("api");
app.MapHub<ChatHub>("/chatHub");
app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok("MyAPI running"));

app.Run();