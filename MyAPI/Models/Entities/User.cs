using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserProfile? Profile { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}