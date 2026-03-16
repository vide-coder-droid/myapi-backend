using MyAPI.Models.Entities;

namespace MyAPI.Repositories
{
    public interface IProfileRepository
    {
        Task<User?> GetUserWithProfileAsync(string username);

        Task SaveAsync();
    }
}