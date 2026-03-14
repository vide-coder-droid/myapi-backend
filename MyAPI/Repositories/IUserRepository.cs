using MyAPI.Models.Entities;

namespace MyAPI.Repositories
{
    public interface IUserRepository
    {
        User GetUser(string username, string password);
        User? GetByUsername(string username);

        void AddUser(User user, string roleName);
        void DeleteUser(User user);
    }
}