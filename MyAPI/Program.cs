using Microsoft.EntityFrameworkCore;
using MyAPI.Extensions;
using MyAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// CHỈ chạy port 8080 khi deploy
if (Environment.GetEnvironmentVariable("RENDER") != null)
{
    builder.WebHost.UseUrls("http://0.0.0.0:8080");
}

// ================= SERVICES =================

builder.Services.AddControllers();

// gọi services từ Extensions
builder.Services.AddApplicationServices();

// 🔹 Add PostgreSQL DB
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connection = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrEmpty(connection))
        throw new Exception("DATABASE_URL not found");

    options.UseNpgsql(connection);
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ================= MIDDLEWARE =================

// BẬT SWAGGER LUÔN
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();