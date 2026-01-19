using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Timetable;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimetablesController : ControllerBase
{
    private readonly ITimetableService _timetableService;

    public TimetablesController(ITimetableService timetableService)
    {
        _timetableService = timetableService;
    }

    [HttpPost]
    public async Task<ActionResult<TimetableResponseDto>> CreateTimetable([FromBody] CreateTimetableDto createTimetableDto)
    {
        var timetable = await _timetableService.CreateTimetableAsync(createTimetableDto);
        return CreatedAtAction(nameof(GetTimetableById), new { id = timetable.Id }, timetable);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TimetableResponseDto>> UpdateTimetable(Guid id, [FromBody] UpdateTimetableDto updateTimetableDto)
    {
        if (id != updateTimetableDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var timetable = await _timetableService.UpdateTimetableAsync(updateTimetableDto);
            return Ok(timetable);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TimetableResponseDto>> GetTimetableById(Guid id)
    {
        var timetable = await _timetableService.GetTimetableByIdAsync(id);
        if (timetable == null)
            return NotFound();

        return Ok(timetable);
    }

    [HttpGet("program/{programId}")]
    public async Task<ActionResult<List<TimetableResponseDto>>> GetTimetableByProgram(
        Guid programId,
        [FromQuery] string? semester = null)
    {
        var timetables = await _timetableService.GetTimetableByProgramAsync(programId, semester);
        return Ok(timetables);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTimetable(Guid id)
    {
        var result = await _timetableService.DeleteTimetableAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
