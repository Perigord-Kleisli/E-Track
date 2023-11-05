using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ETrack.Models.Dtos;

namespace ETrack.Api.Entities
{


    public class User
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
        public List<Student> Students {get; set; } = new List<Student>();
        public List<Student> Children { get; set; } = new List<Student>();
    }
}