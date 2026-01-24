using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Program;
using UniversityManagement.Application.DTOs.Student;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentResponseDto>> CreateStudent([FromBody] CreateStudentDto createStudentDto)
    {
        var student = await _studentService.CreateStudentAsync(createStudentDto);
        return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponseDto>> UpdateStudent(Guid id, [FromBody] UpdateStudentDto updateStudentDto)
    {
        if (id != updateStudentDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var student = await _studentService.UpdateStudentAsync(updateStudentDto);
            return Ok(student);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponseDto>> GetStudentById(Guid id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<StudentResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StudentResponseDto>>> GetAllStudents()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteStudent(Guid id)
    {
        var result = await _studentService.DeleteStudentAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("admit-to-program")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AdmitStudentToProgram([FromBody] AdmitStudentToProgramDto admitDto)
    {
        var result = await _studentService.AdmitStudentToProgramAsync(admitDto);
        if (!result)
            return BadRequest("Student or program not found, or already admitted");

        return Ok(new { message = "Student admitted to program successfully" });
    }

    [HttpGet("{id}/programs")]
    [ProducesResponseType(typeof(List<ProgramResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProgramResponseDto>>> GetStudentPrograms(Guid id)
    {
        var programs = await _studentService.GetStudentProgramsAsync(id);
        return Ok(programs);
    }
}
