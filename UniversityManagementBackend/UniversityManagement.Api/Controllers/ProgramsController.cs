using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Program;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProgramsController : ControllerBase
{
    private readonly IProgramService _programService;

    public ProgramsController(IProgramService programService)
    {
        _programService = programService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProgramResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProgramResponseDto>> CreateProgram([FromBody] CreateProgramDto createProgramDto)
    {
        var program = await _programService.CreateProgramAsync(createProgramDto);
        return CreatedAtAction(nameof(GetProgramById), new { id = program.Id }, program);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProgramResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProgramResponseDto>> UpdateProgram(Guid id, [FromBody] UpdateProgramDto updateProgramDto)
    {
        if (id != updateProgramDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var program = await _programService.UpdateProgramAsync(updateProgramDto);
            return Ok(program);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProgramResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProgramResponseDto>> GetProgramById(Guid id)
    {
        var program = await _programService.GetProgramByIdAsync(id);
        if (program == null)
            return NotFound();

        return Ok(program);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProgramResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProgramResponseDto>>> GetAllPrograms()
    {
        var programs = await _programService.GetAllProgramsAsync();
        return Ok(programs);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProgram(Guid id)
    {
        var result = await _programService.DeleteProgramAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
