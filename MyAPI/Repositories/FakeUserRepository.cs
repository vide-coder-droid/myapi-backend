using MyAPI.Models;
using BCrypt.Net;

namespace MyAPI.Repositories
{
    public class FakeUserRepository : IUserRepository
    {
        private List<User> users = new List<User>()
        {
            new User
            {
                Id = 1,
                Username = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("123456")
            },
            new User
            {
                Id = 2,
                Username = "test",
                Password = BCrypt.Net.BCrypt.HashPassword("password")
            }
        };

        public User GetUser(string username, string password)
        {
            var user = users.FirstOrDefault(x => x.Username == username);

            if (user == null)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            return isValid ? user : null;
        }
    }
}