using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Extensions;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Render port
if (Environment.GetEnvironmentVariable("RENDER") != null)
{
    builder.WebHost.UseUrls("http://0.0.0.0:8080");
}

// Services
builder.Services.AddControllers();
builder.Services.AddApplicationServices();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connection);
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health Check
builder.Services.AddHealthChecks();

// Rate Limit
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

// Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// ===== Auto DB Migration + Seed =====

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();

    DbSeeder.Seed(db);
}

app.Run();