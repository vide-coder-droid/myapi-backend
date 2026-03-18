using MyAPI.Models.Entities;

namespace MyAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string username);

        Task<User?> GetByEmailAsync(string email);

        Task AddUserAsync(User user, string role);

        Task DisableUserAsync(User user);

        Task AddUserDeviceAsync(UserDevice device);
        Task<UserDevice?> GetUserDeviceByRefreshTokenAsync(string refreshToken);
        Task<IEnumerable<UserDevice>> GetUserDevicesAsync(Guid userId);
        Task<UserDevice?> GetDeviceByIdAsync(Guid id);
        Task RevokeDeviceAsync(UserDevice device);

        Task<UserDevice?> GetUserDeviceByUserAndDeviceAsync(Guid userId, string deviceName, string ip);
        Task UpdateUserDeviceAsync(UserDevice device);
    }
}