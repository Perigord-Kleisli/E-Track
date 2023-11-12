using ETrack.Api.Extensions;
using ETrack.Api.Repositories.Contracts;
using ETrack.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ETrack.Api.Controllers
{

    [Route("users/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IAuthRepository authRepository;

        public UserController(IAuthRepository authRepository)
        {
            this.authRepository = authRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await authRepository.GetUsers();
            return Ok(users.Select(x => x.ConvertToDto()));
        }
    }
}