using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.Requests
{
    public class CreateUserRequest
    {
        [Required]
        [MinLength(4)]
        public required string Username { get; set; }

        [Required]
        [MinLength(8)]
        public required string Password { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Role { get; set; }
    }
}