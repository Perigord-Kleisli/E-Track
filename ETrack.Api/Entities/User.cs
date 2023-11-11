using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ETrack.Models.Dtos;

namespace ETrack.Api.Entities
{
    public class UserRegisterToken
    {
        public int Id { get; set; }
        public required Role Role { get; set; }
        public required string Uid { get; set; }
        public required DateTime CreationDate { get; set; }
    }

    public class UserEmailConfirmationToken
    {
        public int Id { get; set; }
        public required Guid Guid { get; set; }
        public int UserId { get; set; }
        //Email Confirmation token expires in 1 hour
        public required DateTime CreationDate { get; set; }
    }

    public class UserPasswordForgotToken 
    {
        public int Id { get; set; }
        public required Guid Guid { get; set; }
        public int UserId { get; set; }
        //Password forgot token expires in 1 hour
        public DateTime CreationDate { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public required string PasswordHash { get; set; }
        public required Role Roles {get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public required DateTime BirthDate { get; set; }
        public required DateTime CreationDate { get; set; }
        public List<Student> Students {get; set; } = new List<Student>();
        public List<Student> Children { get; set; } = new List<Student>();
    }
}