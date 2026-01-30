using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    // Store HASH, not raw token
    [Required, MaxLength(255)]
    public string Token { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

   

    public bool IsActive => RevokedAtUtc == null && DateTime.UtcNow < ExpiresAtUtc;
}
