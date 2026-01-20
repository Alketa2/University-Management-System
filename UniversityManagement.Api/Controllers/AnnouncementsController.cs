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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AnnouncementResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AnnouncementResponseDto>> CreateAnnouncement(
        [FromBody] CreateAnnouncementDto dto)
    {
        var announcement = await _announcementService.CreateAnnouncementAsync(dto);

        return CreatedAtAction(
            nameof(GetAnnouncementById),
            new { id = announcement.Id },
            announcement
        );
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
        var announcements = await _announcementService
            .GetActiveAnnouncementsAsync(programId, subjectId);

        return Ok(announcements);
    }

    [HttpGet("teacher/{teacherId}")]
    [ProducesResponseType(typeof(List<AnnouncementResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AnnouncementResponseDto>>> GetAnnouncementsByTeacher(
        Guid teacherId)
    {
        var announcements = await _announcementService
            .GetAnnouncementsByTeacherAsync(teacherId);

        return Ok(announcements);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAnnouncement(Guid id)
    {
        var deleted = await _announcementService.DeleteAnnouncementAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
