using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Entities
{
    public class AppUser : BaseEntity
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Role { get; set; } = "User"; // "Admin" or "User"

        public bool IsActive { get; set; } = true;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
