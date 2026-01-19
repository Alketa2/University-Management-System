using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Announcement;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;

    public AnnouncementsController(IAnnouncementService announcementService)
    {
        _announcementService = announcementService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AnnouncementResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AnnouncementResponseDto>> CreateAnnouncement([FromBody] CreateAnnouncementDto createAnnouncementDto)
    {
        var announcement = await _announcementService.CreateAnnouncementAsync(createAnnouncementDto);
        return CreatedAtAction(nameof(GetAnnouncementById), new { id = announcement.Id }, announcement);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AnnouncementResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnnouncementResponseDto>> UpdateAnnouncement(Guid id, [FromBody] UpdateAnnouncementDto updateAnnouncementDto)
    {
        if (id != updateAnnouncementDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var announcement = await _announcementService.UpdateAnnouncementAsync(updateAnnouncementDto);
            return Ok(announcement);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AnnouncementResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnnouncementResponseDto>> GetAnnouncementById(Guid id)
    {
        var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
        if (announcement == null)
            return NotFound();

        return Ok(announcement);
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(List<AnnouncementResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AnnouncementResponseDto>>> GetActiveAnnouncements(
        [FromQuery] Guid? programId = null,
        [FromQuery] Guid? subjectId = null)
    {
        var announcements = await _announcementService.GetActiveAnnouncementsAsync(programId, subjectId);
        return Ok(announcements);
    }

    [HttpGet("teacher/{teacherId}")]
    [ProducesResponseType(typeof(List<AnnouncementResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AnnouncementResponseDto>>> GetAnnouncementsByTeacher(Guid teacherId)
    {
        var announcements = await _announcementService.GetAnnouncementsByTeacherAsync(teacherId);
        return Ok(announcements);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAnnouncement(Guid id)
    {
        var result = await _announcementService.DeleteAnnouncementAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
