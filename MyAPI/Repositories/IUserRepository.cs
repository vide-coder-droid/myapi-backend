using MyAPI.Models.Entities;

namespace MyAPI.Repositories
{
    public interface IUserRepository
    {
        User GetUser(string username, string password);
    }
}