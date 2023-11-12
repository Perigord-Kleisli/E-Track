using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DotNet.RateLimiter.ActionFilters;
using ETrack.Api.Entities;
using ETrack.Api.Repositories.Contracts;
using ETrack.Api.Services;
using ETrack.Api.Services.Contracts;
using ETrack.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
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
        private readonly IEmailService emailService;

        public AuthController( IAuthRepository authRepository, IConfiguration configuration, IEmailService emailService)
        {
            this.authRepository = authRepository;
            this.configuration = configuration;
            this.emailService = emailService;
        }

        [HttpPost("register")]
        [RateLimit(PeriodInSec = 60, Limit = 1)]
        public async Task<ActionResult<User>> Register(UserRegisterDto request) {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            Role roles; 
            try 
            {
                roles = await authRepository.GetTokenAsync(request.RegisterToken);
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

            if (!await authRepository.addUserAsync(user))
            {
                return BadRequest($"'{request.Email}' is already taken");
            }

            return Ok(user);
        }

        [HttpPost("reset-password-token")]
        [RateLimit(PeriodInSec = 90, Limit = 3)]
        public async Task<ActionResult> GetPasswordToken([EmailAddress] string email) {
            var user =  await authRepository.GetByUserByEmailAsync(email);

            if (user is null)
                return BadRequest($"User email '{email}' not found");
            if (!user.IsEmailConfirmed)
                return BadRequest("User email is not confirmed");

            var resetPasswordToken = await authRepository.CreatePasswordForgotTokenAsync(user);

            var apiAddress = new Uri (Request.GetDisplayUrl());
            var baseUri = apiAddress.GetLeftPart(System.UriPartial.Authority);

            emailService.sendEmail(new EmailDto {
                To = email,
                Subject = "ETrack password reset",
                Body =  emailResetPage($"{baseUri}/reset-password.html?password-reset-token={resetPasswordToken}")
            });
            return Ok();
        }


        [HttpPost("confirmation-token")]
        [RateLimit(PeriodInSec = 90, Limit = 3)]
        public async Task<ActionResult> GetConfirmationToken([EmailAddress] string emailToVerify) {

            var user =  await authRepository.GetByUserByEmailAsync(emailToVerify);

            if (user is null)
                return BadRequest($"User email '{emailToVerify}' not found");
            if (user.IsEmailConfirmed)
                return BadRequest("User is already confirmed");

            var confirmationToken =  await authRepository.CreateConfirmationTokenAsync(user);

            var apiAddress = new Uri (Request.GetDisplayUrl());
            var baseUri = apiAddress.GetLeftPart(System.UriPartial.Authority);

            emailService.sendEmail(new EmailDto {
                To = emailToVerify,
                Subject = "ETrack verification",
                Body =  emailConfirmPage($"{baseUri}/confirmation.html?confirmation-token={confirmationToken}")
            });
            return Ok();
        }

        [HttpPost("confirm-email")]
        public async Task<ActionResult> ConfirmEmail(Guid confirmationGUID) 
        {
            try 
            {
                await authRepository.UseConfirmationTokenAsync(confirmationGUID);
                return Ok();
            } 
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto request) {
            try 
            {
                await authRepository.UsePasswordForgotTokenAsync(request.guid, request.password);
                return Ok();
            } 
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("generate-register-token"), Authorize(Roles=RoleType.ParentOrTeacher)]
        public async Task<ActionResult<string>> GenerateRegisterToken(CreateTokenDto request)
        {
            Role flag = 0b000;
            if (request.isParent) flag = flag | Role.Parent;
            if (request.isTeacher) flag = flag | Role.Teacher;
            if (request.isAdmin) flag = flag | Role.Admin;

            var token  = await authRepository.GenTokenAsync(flag);
            return Ok(token.Uid);
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request) {
            var user = await authRepository.GetByUserByEmailAsync(request.Email);
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

        private string emailConfirmPage(string href) => $$"""
        <!DOCTYPE html>
        <html lang="en">

        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <link href='https://fonts.googleapis.com/css?family=Fira Sans' rel='stylesheet'>
            <style>
                :root {
                    --secondary-color: #8F4E8B;
                    --secondary-color-light: #a75fa2;
                    --secondary-color-dark: #743b70;
                    --primary-color: #5E5086;
                }

                .confirm-btn {
                    margin-top: 1em;
                    border-radius: 0.5rem;
                    padding: 1em;
                    font-weight: bold;
                    font-size: larger;
                    color: white;
                    border: 0px;
                    text-decoration: none;
                    background-color: var(--secondary-color);
                    cursor: pointer;
                }

                .confirm-btn:active {
                    background-color: var(--secondary-color-light);
                }

                .confirm-btn:hover {
                    background-color: var(--secondary-color-dark);
                }

                body {
                    background-color: white;
                    padding: 1em;
                    border-radius: 1em;
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    padding: 2em;
                    font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
                }

                h1 {
                    font-size: 4em;
                    font-family: 'Fira Sans';
                    font-weight: bolder;
                    margin: 0;
                }

                .logo {
                    margin-top: 2em;
                }

                hr {
                    color: 2px solid gray;
                    width: 95%;
                }

                p {
                    padding: 1em;
                    font-size: large;
                    text-align: center;
                }
            </style>
        </head>

        <body>
                <svg class="logo" width="150px" height="150px" viewBox="0 0 154.77618 181.42081" version="1.1" id="svg1"
                    sodipodi:docname="icon.svg" inkscape:export-filename="/home/truff/.local/src/e-track/wwwroot/icon.png"
                    inkscape:export-xdpi="96" inkscape:export-ydpi="96" inkscape:version="1.1.2 (0a00cf5339, 2022-02-04)"
                    xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
                    xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd" xmlns="http://www.w3.org/2000/svg"
                    xmlns:svg="http://www.w3.org/2000/svg">
                    <sodipodi:namedview id="namedview1" pagecolor="#ffffff" bordercolor="#000000" borderopacity="0.25"
                        inkscape:showpageshadow="2" inkscape:pageopacity="0.0" inkscape:pagecheckerboard="0"
                        inkscape:deskcolor="#d1d1d1" inkscape:document-units="mm" inkscape:pageshadow="2" showgrid="false"
                        inkscape:zoom="0.78683728" inkscape:cx="653.88361" inkscape:cy="369.83504" inkscape:window-width="1920"
                        inkscape:window-height="935" inkscape:window-x="0" inkscape:window-y="32" inkscape:window-maximized="1"
                        inkscape:current-layer="layer1" />
                    <defs id="defs1" />
                    <g inkscape:label="Layer 1" inkscape:groupmode="layer" id="layer1"
                        transform="translate(-27.611912,-57.789622)">
                        <g id="g13" transform="matrix(3.1565396,0,0,3.1565396,-205.71278,-322.9835)"
                            inkscape:export-filename="icon.svg" inkscape:export-xdpi="5.2514539"
                            inkscape:export-ydpi="5.2514539">
                            <path id="path8"
                                style="font-weight:bold;font-size:192px;font-family:'Noto Sans Math';-inkscape-font-specification:'Noto Sans Math Bold';letter-spacing:-36px;white-space:pre;fill:#8f4e8b;stroke-width:0.448231"
                                d="m 83.384614,126.48349 a 28.737258,28.737258 0 0 0 -9.466741,12.5234 v 20.72064 a 28.737258,28.737258 0 0 0 9.466741,12.52339 v -19.44774 h 10.757364 v -8.60642 H 83.384614 Z" />
                            <path id="path6" style="fill:#5e5086;fill-opacity:1;stroke-width:0.268534"
                                d="m 100.64647,120.62992 a 28.737258,28.737258 0 0 0 -4.97473,0.48506 v 56.50446 a 28.737258,28.737258 0 0 0 4.97473,0.48506 28.737258,28.737258 0 0 0 4.86963,-0.46211 v -17.52395 l 2.926,-5.50829 7.34369,19.14941 a 28.737258,28.737258 0 0 0 7.16558,-6.35714 l -8.74332,-22.17243 7.53563,-15.36699 a 28.737258,28.737258 0 0 0 -6.72172,-5.37008 l -9.50586,20.1344 v -23.5731 a 28.737258,28.737258 0 0 0 -4.86963,-0.4243 z" />
                        </g>
                    </g>
                </svg>
                <h1>E-Track</h1>
                <hr />
                <p>
                    You have created an E-Track account with this email address. If this wasn't you, please ignore this email
                </p>
                <a href="{{href}}" target="_blank" class="confirm-btn">
                    Verify email address
                </a>
        </body>
        </html>
        """;
        private string emailResetPage(string href) => $$"""
        <!DOCTYPE html>
        <html lang="en">

        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <link href='https://fonts.googleapis.com/css?family=Fira Sans' rel='stylesheet'>
            <style>
                :root {
                    --secondary-color: #8F4E8B;
                    --secondary-color-light: #a75fa2;
                    --secondary-color-dark: #743b70;
                    --primary-color: #5E5086;
                }

                .confirm-btn {
                    margin-top: 1em;
                    border-radius: 0.5rem;
                    padding: 1em;
                    font-weight: bold;
                    font-size: larger;
                    color: white;
                    border: 0px;
                    text-decoration: none;
                    background-color: var(--secondary-color);
                    cursor: pointer;
                }

                .confirm-btn:active {
                    background-color: var(--secondary-color-light);
                }

                .confirm-btn:hover {
                    background-color: var(--secondary-color-dark);
                }

                body {
                    background-color: white;
                    padding: 1em;
                    border-radius: 1em;
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    padding: 2em;
                    font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
                }

                h1 {
                    font-size: 4em;
                    font-family: 'Fira Sans';
                    font-weight: bolder;
                    margin: 0;
                }

                .logo {
                    margin-top: 2em;
                }

                hr {
                    color: 2px solid gray;
                    width: 95%;
                }

                p {
                    padding: 1em;
                    font-size: large;
                    text-align: center;
                }
            </style>
        </head>

        <body>
            <svg class="logo" width="150px" height="150px" viewBox="0 0 154.77618 181.42081" version="1.1" id="svg1"
                sodipodi:docname="icon.svg" inkscape:export-filename="/home/truff/.local/src/e-track/wwwroot/icon.png"
                inkscape:export-xdpi="96" inkscape:export-ydpi="96" inkscape:version="1.1.2 (0a00cf5339, 2022-02-04)"
                xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
                xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd" xmlns="http://www.w3.org/2000/svg"
                xmlns:svg="http://www.w3.org/2000/svg">
                <sodipodi:namedview id="namedview1" pagecolor="#ffffff" bordercolor="#000000" borderopacity="0.25"
                    inkscape:showpageshadow="2" inkscape:pageopacity="0.0" inkscape:pagecheckerboard="0"
                    inkscape:deskcolor="#d1d1d1" inkscape:document-units="mm" inkscape:pageshadow="2" showgrid="false"
                    inkscape:zoom="0.78683728" inkscape:cx="653.88361" inkscape:cy="369.83504" inkscape:window-width="1920"
                    inkscape:window-height="935" inkscape:window-x="0" inkscape:window-y="32" inkscape:window-maximized="1"
                    inkscape:current-layer="layer1" />
                <defs id="defs1" />
                <g inkscape:label="Layer 1" inkscape:groupmode="layer" id="layer1" transform="translate(-27.611912,-57.789622)">
                    <g id="g13" transform="matrix(3.1565396,0,0,3.1565396,-205.71278,-322.9835)"
                        inkscape:export-filename="icon.svg" inkscape:export-xdpi="5.2514539" inkscape:export-ydpi="5.2514539">
                        <path id="path8"
                            style="font-weight:bold;font-size:192px;font-family:'Noto Sans Math';-inkscape-font-specification:'Noto Sans Math Bold';letter-spacing:-36px;white-space:pre;fill:#8f4e8b;stroke-width:0.448231"
                            d="m 83.384614,126.48349 a 28.737258,28.737258 0 0 0 -9.466741,12.5234 v 20.72064 a 28.737258,28.737258 0 0 0 9.466741,12.52339 v -19.44774 h 10.757364 v -8.60642 H 83.384614 Z" />
                        <path id="path6" style="fill:#5e5086;fill-opacity:1;stroke-width:0.268534"
                            d="m 100.64647,120.62992 a 28.737258,28.737258 0 0 0 -4.97473,0.48506 v 56.50446 a 28.737258,28.737258 0 0 0 4.97473,0.48506 28.737258,28.737258 0 0 0 4.86963,-0.46211 v -17.52395 l 2.926,-5.50829 7.34369,19.14941 a 28.737258,28.737258 0 0 0 7.16558,-6.35714 l -8.74332,-22.17243 7.53563,-15.36699 a 28.737258,28.737258 0 0 0 -6.72172,-5.37008 l -9.50586,20.1344 v -23.5731 a 28.737258,28.737258 0 0 0 -4.86963,-0.4243 z" />
                    </g>
                </g>
            </svg>
            <h1>E-Track</h1>
            <hr />
            <p>
                You have asked to ret your E-Track account password with this email address. If this wasn't you, please ignore this email
            </p>
            <a href="{{href}}" target="_blank" class="confirm-btn">
                Reset Password
            </a>
        </body>
        </html>
        """;
    }

}
