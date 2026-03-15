using MyAPI.Data;
using MyAPI.Extensions;
using Scalar.AspNetCore;
using Serilog;
using MyAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Port
builder.ConfigurePort();

// Logging
builder.ConfigureLogging();

// Services
builder.Services.AddControllers();
builder.Services.AddApplicationServices();

// Infrastructure
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocs();
builder.Services.AddRateLimiterConfig();
builder.Services.AddCorsPolicy();
builder.Services.AddHealthChecks();
builder.Services.AddSignalRConfig();

var app = builder.Build();

// ===== APPLY MIGRATION FIRST =====
app.ApplyMigration();

// Middleware
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
app.UseAuthorization();

// Routes
app.MapControllers().RequireRateLimiting("api");
app.MapHub<ChatHub>("/chatHub");
app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok("MyAPI running"));

app.Run();