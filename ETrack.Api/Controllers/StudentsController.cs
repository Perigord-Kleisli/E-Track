using System.Security.Claims;
using ETrack.Api.Entities;
using ETrack.Api.Extensions;
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
        private readonly IAuthRepository authRepository;

        public StudentsController(IStudentRepository studentRepository, IConfiguration configuration, IAuthRepository authRepository)
        {
            this.studentRepository = studentRepository;
            this.configuration = configuration;
            this.authRepository = authRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<SimpleStudentDto>>> GetStudents()
        {
            var students = await studentRepository.GetStudentsAsync();
            if (students is null) return BadRequest();
            return Ok(students.Select(x => new SimpleStudentDto
            {
                Name = x.Name,
                Grade = x.Grade,
                Id = x.Id
            }));
        }

        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<StudentDto>> GetStudents(int id)
        {
            var user = await authRepository.GetUserAsync(int.Parse(getUserClaim(ClaimTypes.Sid)));
            if (!(user!.Children.Any(x => x.Id == id) 
                 || user.Students.Any(x => x.Id == id)))
            {
                return BadRequest($"User lacks permission to view student info");
            }

            var student = await studentRepository.getStudentByIdAsync(id);
            if (student is null)
            {
                return BadRequest($"Student {id} not found");
            }

            return Ok(student.ConvertToDto());
        }

        [HttpPost, Authorize(Roles = RoleType.Teacher)]
        public async Task<ActionResult> CreateStudentAsync(AddStudentDto addStudentDto)
        {
            var student = new Student
            {
                Name = addStudentDto.Name,
                Grade = addStudentDto.Grade,
            };
            await studentRepository.addStudentAsync(student);
            return Ok();
        }

        [HttpPost("sections"), Authorize(Roles = RoleType.Teacher)]
        public async Task<ActionResult> CreateSectionAsync(AddSectionDTO addSectionDTO) 
        {
            var adviser = await authRepository.GetUserAsync(addSectionDTO.AdviserId);
            if (adviser is null)
                return BadRequest("Advisor does not exist");

            List<Student> students = new List<Student>{};
            
            var section = new Section {
                Name = addSectionDTO.Name,
                Grade = addSectionDTO.Grade,
                Adviser = adviser,
                Students = new List<Student>{},
                // Schedule = new List<Schedule>{} //Did not have enough time to implement schedules
            };

            await studentRepository.addSectionAsync(section);
            return Ok();
        }

        [HttpGet("sections")]
        public async Task<ActionResult<IEnumerable<SectionDto>>> GetSections()
        {
            var sections = await studentRepository.GetSectionsAsync();
            return Ok(sections!.Select(x => x.ConvertToDto()));
        }

        private string getUserClaim(string claimType)
        {
            return HttpContext.User.Claims.First(x => x.Type == claimType).Value;
        }

    }

}