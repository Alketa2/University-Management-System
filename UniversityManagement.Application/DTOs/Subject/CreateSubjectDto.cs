using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Subjects
{
    public class CreateSubjectDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(1, 60)]
        public int Credits { get; set; }

        [Required]
        public Guid ProgramId { get; set; }

        [Required]
        public Guid TeacherId { get; set; }

        [Range(1, 12)]
        public int Semester { get; set; }
    }
}
