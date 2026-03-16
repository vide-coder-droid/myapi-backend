using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.Requests
{
    public class LoginRequest
    {
        [Required]
        [MinLength(4)]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}