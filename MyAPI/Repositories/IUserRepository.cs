using MyAPI.Models;

namespace MyAPI.Repositories
{
    public interface IUserRepository
    {
        User GetUser(string username, string password);
    }
}