using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Grade;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public GradesController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    [HttpPost]
    [Authorize(Policy = "RequireTeacherOrAdmin")]
    [ProducesResponseType(typeof(GradeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GradeResponseDto>> CreateGrade([FromBody] CreateGradeDto createGradeDto)
    {
        var grade = await _gradeService.CreateGradeAsync(createGradeDto);
        return CreatedAtAction(nameof(GetGradeById), new { id = grade.Id }, grade);
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

        try
        {
            var grade = await _gradeService.UpdateGradeAsync(updateGradeDto);
            return Ok(grade);
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
