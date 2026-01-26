using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Domain.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(80)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(80)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        // store password hash 
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        //  role for now: "Admin" | "Teacher" | "Student"
        [Required, MaxLength(30)]
        public string Role { get; set; } = "Student";

        public List<RefreshToken> RefreshTokens { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
