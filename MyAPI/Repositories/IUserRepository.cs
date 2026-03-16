using MyAPI.Models.Entities;

namespace MyAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string username);

        Task<User?> GetByEmailAsync(string email);

        Task AddUserAsync(User user, string role);

        Task DisableUserAsync(User user);
    }
}