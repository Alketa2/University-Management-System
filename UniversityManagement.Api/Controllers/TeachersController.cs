using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Teacher;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TeacherResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeacherResponseDto>> CreateTeacher([FromBody] CreateTeacherDto createTeacherDto)
    {
        var teacher = await _teacherService.CreateTeacherAsync(createTeacherDto);
        return CreatedAtAction(nameof(GetTeacherById), new { id = teacher.Id }, teacher);
    }

    [HttpPut("{id}")]

    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TeacherResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(TeacherResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponseDto>> GetTeacherById(Guid id)
    {
        var teacher = await _teacherService.GetTeacherByIdAsync(id);
        if (teacher == null)
            return NotFound();

        return Ok(teacher);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TeacherResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TeacherResponseDto>>> GetAllTeachers()
    {
        var teachers = await _teacherService.GetAllTeachersAsync();
        return Ok(teachers);
    }

    [HttpDelete("{id}")]

    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTeacher(Guid id)
    {
        var result = await _teacherService.DeleteTeacherAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
