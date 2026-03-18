using MyAPI.Models.Entities;

namespace MyAPI.Repositories
{
    public interface IUserRepository
    {
        // ===== User operations =====
        
        Task<User?> GetUserAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task AddUserAsync(User user, string role);
        Task DisableUserAsync(User user);

        // ===== UserDevice operations =====

        // Add new device
        Task AddUserDeviceAsync(UserDevice device);

        // Get device by refresh token
        Task<UserDevice?> GetUserDeviceByRefreshTokenAsync(string refreshToken);

        // Get all devices for a user
        Task<IEnumerable<UserDevice>> GetUserDevicesAsync(Guid userId);

        // Get device by ID
        Task<UserDevice?> GetDeviceByIdAsync(Guid id);

        // Revoke a device (logout)
        Task RevokeDeviceAsync(UserDevice device);

        // Get device by user + device name + IP
        Task<UserDevice?> GetUserDeviceByUserAndDeviceAsync(Guid userId, string deviceName, string ip);

        // Update existing device
        Task UpdateUserDeviceAsync(UserDevice device);
    }
}