using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using ETrack.Api.Entities;
using ETrack.Api.Repositories.Contracts;
using ETrack.Api.Services;
using ETrack.Api.Services.Contracts;
using ETrack.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;

namespace ETrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;

        public AuthController( IAuthRepository authRepository, IConfiguration configuration, IEmailService emailService)
        {
            this.authRepository = authRepository;
            this.configuration = configuration;
            this.emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegisterDto request) {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            Role roles; 
            try 
            {
                roles = await authRepository.GetToken(request.RegisterToken);
            }   
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }

            var user = new User {
                Email = request.Email,
                FullName = request.FullName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = passwordHash,
                BirthDate = request.BirthDate,
                Roles = roles,
                CreationDate = DateTime.Now, 
            };

            if (!await authRepository.addUser(user))
            {
                return BadRequest($"'{request.Email}' is already taken");
            }

            return Ok(user);
        }

        [HttpPost("confirmation-token")]
        public async Task<ActionResult> GetConfirmationToken([EmailAddress] string emailToVerify) {

            var user =  await authRepository.GetByUserByEmail(emailToVerify);

            if (user is null)
                return BadRequest($"User {emailToVerify} not found");
            if (user.IsEmailConfirmed)
                return BadRequest("User is already confirmed");

            var confirmationToken =  await authRepository.CreateConfirmationToken(user);

            var apiAddress = new Uri (Request.GetDisplayUrl());
            var baseUri = apiAddress.GetLeftPart(System.UriPartial.Authority);

            emailService.sendEmail(new EmailDto {
                To = emailToVerify,
                Subject = "ETrack verification",
                Body = $"""
                <a href="http://localhost:5195/confirm-email/{confirmationToken}">Confirm Email</a>
                """
            });
            return Ok();
        }
        [HttpPost("confirm-email")]
        public async Task<ActionResult> ConfirmEmail(Guid confirmationGUID) 
        {
            try 
            {
                await authRepository.UseConfirmationToken(confirmationGUID);
                return Ok();
            } 
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("generate-register-token"), Authorize(Roles ="Teacher,Admin")]
        public async Task<ActionResult<string>> GenerateRegisterToken(CreateTokenDto request)
        {
            var flag = Role.None;
            if (request.isParent) flag = flag | Role.Parent;
            if (request.isTeacher) flag = flag | Role.Teacher;
            if (request.isAdmin) flag = flag | Role.Admin;

            var token  = await authRepository.GenToken(flag);
            return Ok(token.Uid);
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
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) 
            {
                return BadRequest("Invalid password");
            }
            if (!user.IsEmailConfirmed) 
            {
                return BadRequest("User must have a confirmed email");
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

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes( 
                    configuration["AppSettings:Token"]!));

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