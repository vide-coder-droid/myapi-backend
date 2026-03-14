using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using BCrypt.Net;
using MyAPI.Models.Entities;

namespace MyAPI.Repositories
{
    public class DbUserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public DbUserRepository(AppDbContext db)
        {
            _db = db;
        }

        public User GetUser(string username, string password)
        {
            var user = _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(x => x.Username == username);

            if (user == null)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return isValid ? user : null;
        }
    }
}