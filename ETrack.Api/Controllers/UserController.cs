using System.Security.Claims;
using ETrack.Api.Extensions;
using ETrack.Api.Repositories.Contracts;
using ETrack.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETrack.Api.Controllers
{

    [Route("users/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IAuthRepository authRepository;
        private readonly IStudentRepository studentRepository;

        public UserController(IAuthRepository authRepository, IStudentRepository studentRepository)
        {
            this.authRepository = authRepository;
            this.studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await authRepository.GetUsers();
            return Ok(users.Select(x => x.ConvertToDto()));
        }

        [HttpGet("children"), Authorize(Roles = RoleType.Parent)]
        public ActionResult<IEnumerable<StudentDto>> GetChildren()
        {
            var user = authRepository.GetUser(int.Parse(getUserClaim(ClaimTypes.Sid)))!;
            return Ok(user.Children.Select(x => x.ConvertToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUsers(int id)
        {
            var users = await authRepository.GetUserAsync(id);
            if (User is null) {
                return BadRequest("User does not exist");
            }
            return Ok(users!.ConvertToDto());
        }

        private string getUserClaim(string claimType)
        {
            return HttpContext.User.Claims.First(x => x.Type == claimType).Value;
        }

    }
}