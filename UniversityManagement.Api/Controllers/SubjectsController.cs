using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var subject = await _service.GetByIdAsync(id);
            if (subject == null) return NotFound("Subject not found");
            return Ok(subject);
        }

        [HttpGet("program/{programId:guid}")]
        public async Task<IActionResult> GetByProgram(Guid programId)
            => Ok(await _service.GetByProgramIdAsync(programId));

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
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
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubjectDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Subject not found");
            return Ok(updated);
        }

       
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound("Subject not found");
            return NoContent();
        }
    }
}
