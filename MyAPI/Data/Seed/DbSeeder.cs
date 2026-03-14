using BCrypt.Net;
using MyAPI.Models.Entities;

namespace MyAPI.Data.Seed
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Users.Any())
                return;

            var users = new List<User>()
            {
                new User
                {
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456")
                },
                new User
                {
                    Username = "test",
                    Password = BCrypt.Net.BCrypt.HashPassword("password")
                }
            };

            db.Users.AddRange(users);
            db.SaveChanges();
        }
    }
}