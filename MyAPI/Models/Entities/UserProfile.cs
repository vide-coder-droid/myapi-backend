using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models.Entities;

public class UserProfile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [MaxLength(100)]
    public string? FullName { get; set; }

    [MaxLength(200)]
    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public User? User { get; set; }
}