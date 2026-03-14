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

        public User? GetByUsername(string username)
        {
            return _db.Users.FirstOrDefault(x => x.Username == username);
        }

        public void AddUser(User user, string roleName)
        {
            _db.Users.Add(user);
            _db.SaveChanges();

            var role = _db.Roles.First(x => x.Name == roleName);

            _db.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            _db.SaveChanges();
        }

        public void DeleteUser(User user)
        {
            _db?.Users.Remove(user);
            _db?.SaveChanges();
        }
    }
}