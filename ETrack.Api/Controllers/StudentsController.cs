using ETrack.Api.Repositories.Contracts;
using ETrack.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ETrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository studentRepository;
        private readonly IConfiguration configuration;

        public StudentsController(IStudentRepository studentRepository, IConfiguration configuration)
        {
            this.studentRepository = studentRepository;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<SimpleStudentDto>>> GetStudents()
        {
            var students = await studentRepository.GetStudents();
            if (students is null) return BadRequest();
            return Ok(students.Select(x => new SimpleStudentDto { 
                Name = x.Name,
                Grade = x.Grade,
                Id = x.Id
            }));
        }

        [HttpPost("add"), Authorize(Roles="Teacher")]
        public async Task<ActionResult> AddStudent(AddStudentDto addStudentDto)
        {
            var student = new Student
            {
                Name = addStudentDto.Name,
                Grade = addStudentDto.Grade,
            };
            await studentRepository.addStudent(student);
            return Ok();
        }

        [HttpGet("byid")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await studentRepository.getStudentById(id);
            if (student is null)
            {
                return BadRequest($"Student {id} not found");
            }

            return Ok(new StudentDto
            {
                Id = student.Id,
                Grade = student.Grade,
                Name = student.Name,
                Attendance = student.Attendance.Select(x => new SchoolDayDto
                {
                    Id = x.Id,
                    Date = x.Date
                }).ToList(),
                Scores = student.Scores.Select(x => new CompletedActivityDto {
                    Id = x.Id,
                    Score = x.Score,
                    Name = x.Name,
                    TotalScore = x.TotalScore,
                    Issuance =  (x.Issuance is null) ? null : new SchoolDayDto {
                        Id = x.IssuanceId,
                        Date = x.Issuance.Date
                    }
                }).ToList()
            });
        }
    }
}