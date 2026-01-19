using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Subject;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpPost]
    public async Task<ActionResult<SubjectResponseDto>> CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
    {
        var subject = await _subjectService.CreateSubjectAsync(createSubjectDto);
        return CreatedAtAction(nameof(GetSubjectById), new { id = subject.Id }, subject);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SubjectResponseDto>> UpdateSubject(Guid id, [FromBody] UpdateSubjectDto updateSubjectDto)
    {
        if (id != updateSubjectDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var subject = await _subjectService.UpdateSubjectAsync(updateSubjectDto);
            return Ok(subject);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubjectResponseDto>> GetSubjectById(Guid id)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id);
        if (subject == null)
            return NotFound();

        return Ok(subject);
    }

    [HttpGet]
    public async Task<ActionResult<List<SubjectResponseDto>>> GetAllSubjects()
    {
        var subjects = await _subjectService.GetAllSubjectsAsync();
        return Ok(subjects);
    }

    [HttpGet("program/{programId}")]
    public async Task<ActionResult<List<SubjectResponseDto>>> GetSubjectsByProgram(Guid programId)
    {
        var subjects = await _subjectService.GetSubjectsByProgramIdAsync(programId);
        return Ok(subjects);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSubject(Guid id)
    {
        var result = await _subjectService.DeleteSubjectAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
