using Microsoft.EntityFrameworkCore;
using MyAPI.Extensions;
using MyAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================

builder.Services.AddControllers();

// gọi services từ Extensions
builder.Services.AddApplicationServices();

// 🔹 Add PostgreSQL DB
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connection = Environment.GetEnvironmentVariable("DATABASE_URL");
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