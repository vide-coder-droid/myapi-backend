using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models.Entities;

namespace MyAPI.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;

        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserWithProfileAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.UserRoles)
                    .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}