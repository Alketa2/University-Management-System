using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Subject;
using UniversityManagement.Application.DTOs.Subjects;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _service;

        public SubjectsController(ISubjectService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<SubjectResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SubjectResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var subject = await _service.GetByIdAsync(id);
            if (subject == null) return NotFound("Subject not found");
            return Ok(subject);
        }

        [HttpGet("program/{programId:guid}")]
        [ProducesResponseType(typeof(List<SubjectResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByProgram(Guid programId)
            => Ok(await _service.GetByProgramIdAsync(programId));

        [HttpPost]
        [Authorize(Policy = "RequireTeacherOrAdmin")]
        [ProducesResponseType(typeof(SubjectResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateSubjectDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "RequireTeacherOrAdmin")]
        [ProducesResponseType(typeof(SubjectResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubjectDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Subject not found");
            return Ok(updated);
        }

       
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "RequireTeacherOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound("Subject not found");
            return NoContent();
        }
    }
}
