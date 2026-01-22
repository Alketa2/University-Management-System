using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Student
{
    public class UpdateStudentDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(120)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(
            @"^\+?[0-9\s\-()]{7,20}$",
            ErrorMessage = "Phone must be digits and may include +, spaces, -, (, )."
        )]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Enrolled";
    }
}
