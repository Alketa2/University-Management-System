using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Attendance;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceResponseDto>> CreateAttendance([FromBody] CreateAttendanceDto createAttendanceDto)
    {
        var attendance = await _attendanceService.CreateAttendanceAsync(createAttendanceDto);
        return CreatedAtAction(nameof(GetAttendanceById), new { id = attendance.Id }, attendance);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<AttendanceResponseDto>>> CreateBulkAttendance([FromBody] BulkAttendanceDto bulkAttendanceDto)
    {
        var attendances = await _attendanceService.CreateBulkAttendanceAsync(bulkAttendanceDto);
        return Ok(attendances);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AttendanceResponseDto>> UpdateAttendance(Guid id, [FromBody] UpdateAttendanceDto updateAttendanceDto)
    {
        if (id != updateAttendanceDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var attendance = await _attendanceService.UpdateAttendanceAsync(updateAttendanceDto);
            return Ok(attendance);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AttendanceResponseDto>> GetAttendanceById(Guid id)
    {
        var attendance = await _attendanceService.GetAttendanceByIdAsync(id);
        if (attendance == null)
            return NotFound();

        return Ok(attendance);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<List<AttendanceResponseDto>>> GetAttendanceByStudent(
        Guid studentId,
        [FromQuery] Guid? subjectId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var attendances = await _attendanceService.GetAttendanceByStudentAsync(studentId, subjectId, startDate, endDate);
        return Ok(attendances);
    }

    [HttpGet("subject/{subjectId}")]
    public async Task<ActionResult<List<AttendanceResponseDto>>> GetAttendanceBySubject(
        Guid subjectId,
        [FromQuery] DateTime date)
    {
        var attendances = await _attendanceService.GetAttendanceBySubjectAsync(subjectId, date);
        return Ok(attendances);
    }
}
