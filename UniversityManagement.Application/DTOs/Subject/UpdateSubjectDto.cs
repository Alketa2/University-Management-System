using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Subjects
{
    public class UpdateSubjectDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(1, 60)]
        public int Credits { get; set; }

        [Range(1, 12)]
        public int Semester { get; set; }
    }
}
