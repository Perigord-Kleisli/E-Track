using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BCrypt.Net;
using ETrack.Api.Entities;
using ETrack.Api.Repositories.Contracts;
using ETrack.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ETrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;
        private readonly IConfiguration configuration;

        public AuthController( IAuthRepository authRepository, IConfiguration configuration)
        {
            this.authRepository = authRepository;
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegisterDto request) {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var roles = await authRepository.GetToken(request.RegisterToken);

            if (roles is null)
            {
                return BadRequest("Register Token Does Not Exist");
            }

            var user = new User {
                Email = request.Email,
                FullName = request.FullName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = passwordHash,
                BirthDate = request.BirthDate,
                Roles = roles.GetValueOrDefault(),
                CreationDate = DateTime.Now, 
            };


            if (!await authRepository.addUser(user))
            {
                return BadRequest($"'{request.Email}' is already taken");
            }

            return Ok(user);
        }
        [HttpPost("registerToken"), Authorize(Roles ="Teacher,Admin")]
        public async Task<ActionResult<Guid>> RegisterToken(CreateTokenDto request)
        {
            var guid = Guid.NewGuid();
            var flag = Role.None;
            if (request.isParent) flag = flag | Role.Parent;
            if (request.isTeacher) flag = flag | Role.Teacher;
            if (request.isAdmin) flag = flag | Role.Admin;

            await authRepository.AddToken(guid, flag);
            return Ok(guid);
        }

        [HttpGet("admintest"), Authorize(Roles = "Admin")]
        public ActionResult<string> TestGetAdmin() 
        {
            var x = HttpContext.User.Claims.Select(x => x.Value).Aggregate("", (x,accum) => x + accum );
            return Ok($"You are an admin {x}");
        }

        [HttpGet("teachertest"), Authorize(Roles = "Teacher")]
        public ActionResult<string> TestGetTeacher() 
        {
            return Ok($"You are a Teacher {HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Sid).Value }");
        }

        [HttpGet("parenttest"), Authorize(Roles = "Parent")]
        public ActionResult<string> TestGetParent() 
        {
            return Ok("You are a Parent");
        }

        [HttpGet("users"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UsersGetDto>>> GetUsers()
        {
            return Ok(await authRepository.GetUsers());
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request) {
            var user = await authRepository.GetByUserByEmail(request.Email);
            if (user is null) 
            {
                return BadRequest($"Email {request.Email} not found");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) {
                return BadRequest("Invalid password");
            }
            return Ok(CreateToken(user));
        }

        private string CreateToken(User user) 
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Sid, user.Id.ToString())
            };

            if (user.Roles.HasFlag(Role.Parent))
                claims.Add(new Claim(ClaimTypes.Role, "Parent"));
            if (user.Roles.HasFlag(Role.Teacher))
                claims.Add(new Claim(ClaimTypes.Role, "Teacher"));
            if (user.Roles.HasFlag(Role.Admin))
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            // This block of code should be replaced by an actual secret
            // management service if it is to be used in an actual setting
            // As of now, creating and managing an AWS key is too much of
            // a hassle for a 1st year project
            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes( 
                    configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMonths(2),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}