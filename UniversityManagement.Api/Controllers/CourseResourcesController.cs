using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.MongoEntities;

namespace UniversityManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CourseResourcesController : ControllerBase
{
    private readonly ICourseResourceService _resourceService;

    public CourseResourcesController(ICourseResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpGet("subject/{subjectId}")]
    public async Task<IActionResult> GetBySubject(string subjectId)
    {
        var resources = await _resourceService.GetSubjectResourcesAsync(subjectId);
        return Ok(resources);
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost]
    public async Task<IActionResult> Create(CourseResource resource)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            resource.UploadedBy = userId ?? "Unknown";
            
            var result = await _resourceService.AddResourceAsync(resource);
            return CreatedAtAction(nameof(GetBySubject), new { subjectId = result.SubjectId }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error adding resource", details = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, CourseResource resource)
    {
        try
        {
            await _resourceService.UpdateResourceAsync(id, resource);
            return Ok(resource);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error updating resource", details = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _resourceService.DeleteResourceAsync(id);
        return NoContent();
    }
}
