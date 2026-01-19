using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Exam;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamsController(IExamService examService)
    {
        _examService = examService;
    }

    [HttpPost]
    public async Task<ActionResult<ExamResponseDto>> CreateExam([FromBody] CreateExamDto createExamDto)
    {
        var exam = await _examService.CreateExamAsync(createExamDto);
        return CreatedAtAction(nameof(GetExamById), new { id = exam.Id }, exam);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ExamResponseDto>> UpdateExam(Guid id, [FromBody] UpdateExamDto updateExamDto)
    {
        if (id != updateExamDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var exam = await _examService.UpdateExamAsync(updateExamDto);
            return Ok(exam);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExamResponseDto>> GetExamById(Guid id)
    {
        var exam = await _examService.GetExamByIdAsync(id);
        if (exam == null)
            return NotFound();

        return Ok(exam);
    }

    [HttpGet]
    public async Task<ActionResult<List<ExamResponseDto>>> GetAllExams()
    {
        var exams = await _examService.GetAllExamsAsync();
        return Ok(exams);
    }

    [HttpGet("subject/{subjectId}")]
    public async Task<ActionResult<List<ExamResponseDto>>> GetExamsBySubject(Guid subjectId)
    {
        var exams = await _examService.GetExamsBySubjectAsync(subjectId);
        return Ok(exams);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteExam(Guid id)
    {
        var result = await _examService.DeleteExamAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
