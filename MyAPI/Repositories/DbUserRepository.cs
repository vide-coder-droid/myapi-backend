using MyAPI.Models;
using MyAPI.Data;
using BCrypt.Net;

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
            var user = _db.Users.FirstOrDefault(x => x.Username == username);

            if (user == null)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            return isValid ? user : null;
        }
    }
}