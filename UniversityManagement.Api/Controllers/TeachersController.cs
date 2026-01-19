using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Teacher;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpPost]
    public async Task<ActionResult<TeacherResponseDto>> CreateTeacher([FromBody] CreateTeacherDto createTeacherDto)
    {
        var teacher = await _teacherService.CreateTeacherAsync(createTeacherDto);
        return CreatedAtAction(nameof(GetTeacherById), new { id = teacher.Id }, teacher);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TeacherResponseDto>> UpdateTeacher(Guid id, [FromBody] UpdateTeacherDto updateTeacherDto)
    {
        if (id != updateTeacherDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var teacher = await _teacherService.UpdateTeacherAsync(updateTeacherDto);
            return Ok(teacher);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeacherResponseDto>> GetTeacherById(Guid id)
    {
        var teacher = await _teacherService.GetTeacherByIdAsync(id);
        if (teacher == null)
            return NotFound();

        return Ok(teacher);
    }

    [HttpGet]
    public async Task<ActionResult<List<TeacherResponseDto>>> GetAllTeachers()
    {
        var teachers = await _teacherService.GetAllTeachersAsync();
        return Ok(teachers);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTeacher(Guid id)
    {
        var result = await _teacherService.DeleteTeacherAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
