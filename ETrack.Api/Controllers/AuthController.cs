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

        [HttpPost("register"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> Register(UserRegisterDto request) {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User {
                Email = request.Email,
                FullName = request.FullName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = passwordHash,
                Roles = request.Permission,
                CreationDate = DateTime.Now
            };

            if (await authRepository.addUser(user))
            {
                return Ok(user);
            }
            else 
            {
                return BadRequest($"'{request.Email}' is already taken");
            }
        }

        [HttpGet("admintest"), Authorize(Roles = "Admin")]
        public ActionResult<string> TestGetAdmin() 
        {
            return Ok("You are an admin");
        }

        [HttpGet("teachertest"), Authorize(Roles = "Teacher")]
        public ActionResult<string> TestGetTeacher() 
        {
            return Ok("You are a Teacher");
        }

        [HttpGet("parenttest"), Authorize(Roles = "Parent")]
        public ActionResult<string> TestGetParent() 
        {
            return Ok("You are a Parent");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request) {
            var user = await authRepository.GetByUserByName(request.Email);
            if (user is null) 
            {
                return BadRequest($"{request.Email} not found");
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