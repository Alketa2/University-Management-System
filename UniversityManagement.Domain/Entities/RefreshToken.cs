using System;
using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        [Required]
        public Guid AppUserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? RevokedAtUtc { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
        public bool IsRevoked => RevokedAtUtc != null;

        // Navigation
        public AppUser AppUser { get; set; } = null!;
    }
}
