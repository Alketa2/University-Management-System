using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversityManagement.Application.DTOs.Grade;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly UniversityDbContext _context;

    public GradesController(IGradeService gradeService, UniversityDbContext context)
    {
        _gradeService = gradeService;
        _context = context;
    }

    [HttpPost]
    [Authorize(Policy = "RequireTeacherOrAdmin")]
    [ProducesResponseType(typeof(GradeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GradeResponseDto>> CreateGrade([FromBody] CreateGradeDto createGradeDto)
    {
        // Reset to null - will be set if valid teacher profile found
        createGradeDto.GradedByTeacherId = null;

        // Try to get teacher ID from logged-in user
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(userEmail))
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == userEmail);
            if (teacher != null)
            {
                createGradeDto.GradedByTeacherId = teacher.Id;
            }
        }

        try
        {
            var grade = await _gradeService.CreateGradeAsync(createGradeDto);
            return CreatedAtAction(nameof(GetGradeById), new { id = grade.Id }, grade);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireTeacherOrAdmin")]
    [ProducesResponseType(typeof(GradeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GradeResponseDto>> UpdateGrade(Guid id, [FromBody] UpdateGradeDto updateGradeDto)
    {
        if (id != updateGradeDto.Id)
            return BadRequest("ID mismatch");

        // Get teacher ID from logged-in user
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(userEmail))
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == userEmail);
            if (teacher != null)
            {
                updateGradeDto.GradedByTeacherId = teacher.Id;
            }
        }

        try
        {
            var grade = await _gradeService.UpdateGradeAsync(updateGradeDto);
            return Ok(grade);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GradeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GradeResponseDto>> GetGradeById(Guid id)
    {
        var grade = await _gradeService.GetGradeByIdAsync(id);
        return grade is null ? NotFound() : Ok(grade);
    }

    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(typeof(List<GradeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GradeResponseDto>>> GetGradesByStudent(Guid studentId)
    {
        // Students can only view their own grades
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole == "Student")
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);
            
            if (student == null || student.Id != studentId)
                return Forbid(); // Students can only access their own grades
        }

        var grades = await _gradeService.GetGradesByStudentAsync(studentId);
        return Ok(grades);
    }

    [HttpGet("subject/{subjectId:guid}")]
    [Authorize(Policy = "RequireTeacherOrAdmin")]
    [ProducesResponseType(typeof(List<GradeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GradeResponseDto>>> GetGradesBySubject(Guid subjectId)
    {
        var grades = await _gradeService.GetGradesBySubjectAsync(subjectId);
        return Ok(grades);
    }

    [HttpGet("exam/{examId:guid}")]
    [Authorize(Policy = "RequireTeacherOrAdmin")]
    [ProducesResponseType(typeof(List<GradeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GradeResponseDto>>> GetGradesByExam(Guid examId)
    {
        var grades = await _gradeService.GetGradesByExamAsync(examId);
        return Ok(grades);
    }

    [HttpGet("gpa/student/{studentId:guid}")]
    [ProducesResponseType(typeof(StudentGPADto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentGPADto>> GetStudentGPA(
        Guid studentId,
        [FromQuery] string? academicYear = null,
        [FromQuery] int? semester = null)
    {
        // Students can only view their own GPA
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole == "Student")
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null || student.Id != studentId)
                return Forbid();
        }

        try
        {
            var gpa = await _gradeService.GetStudentGPAAsync(studentId, academicYear, semester);
            return Ok(gpa);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireTeacherOrAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGrade(Guid id)
    {
        var result = await _gradeService.DeleteGradeAsync(id);
        return result ? NoContent() : NotFound();
    }
}
