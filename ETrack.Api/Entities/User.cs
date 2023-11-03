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
        public required DateTime CreationDate { get; set; }
    }
}