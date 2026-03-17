using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models.Entities;

namespace MyAPI.Repositories
{
    public class DbUserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        private const string DefaultAvatar =
        "https://res.cloudinary.com/devtzdqde/raw/upload/v1773621355/images/93147492-dbc1-404a-9822-86e16ae4dbfa.jpg";

        public DbUserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetUserAsync(string username)
        {
            username = username.ToLower();

            return await _db.Users
                .Include(x => x.Profile)
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(x => x.Username.ToLower() == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task AddUserAsync(User user, string roleName)
        {
            // tìm role
            var role = await _db.Roles
                .FirstOrDefaultAsync(x => x.Name == roleName);

            if (role == null)
                throw new Exception($"Role '{roleName}' not found");

            // thêm user
            await _db.Users.AddAsync(user);

            // tạo profile mặc định
            var profile = new UserProfile
            {
                UserId = user.Id,
                FullName = user.Username,
                AvatarUrl = DefaultAvatar
            };

            await _db.UserProfiles.AddAsync(profile);

            // gán role
            await _db.UserRoles.AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            // lưu
            await _db.SaveChangesAsync();
        }

        public async Task DisableUserAsync(User user)
        {
            user.IsActive = false;

            await _db.SaveChangesAsync();
        }
    }
}