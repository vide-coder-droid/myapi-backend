using Microsoft.EntityFrameworkCore;
using MyAPI.Data;

namespace MyAPI.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.Migrate();
        DbSeeder.Seed(db);
    }
}