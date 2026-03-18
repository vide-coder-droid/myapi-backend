using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;

        public UserProfile? Profile { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();
    }
}