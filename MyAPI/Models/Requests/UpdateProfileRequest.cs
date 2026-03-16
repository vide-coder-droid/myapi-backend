using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.Requests
{
    public class UpdateProfileRequest
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(200)]
        public string? Bio { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTime? Birthday { get; set; }

        public string? Gender { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}