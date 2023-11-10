using System.ComponentModel.DataAnnotations;

namespace ETrack.Models.Dtos
{

    [Flags]
    public enum Role 
    {
        None    = 0b000,
        Parent  = 0b100,
        Teacher = 0b010,
        Admin   = 0b001,
    }

    public class UserLoginDto
    {
       [EmailAddress]
       public required string Email { get; set; }
       [MinLength(8)]
       public required string Password { get; set; }
    }

    public class UserRegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string RegisterToken { get; set; }
        public required DateTime BirthDate {get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    public class UsersGetDto
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public required string PasswordHash { get; set; }
        public Role Roles {get; set; } = Role.None;
        public bool IsEmailConfirmed { get; set; } = false;
        public required DateTime BirthDate { get; set; }
        public required DateTime CreationDate { get; set; }
    }

    public class ResetPasswordDto 
    {
        public Guid guid { get; set; }
        public required string password { get; set; }
    }

    public class CreateTokenDto
    {
        public bool isParent { get; set; }     
        public bool isTeacher { get; set; }     
        public bool isAdmin { get; set; }     
    }
}