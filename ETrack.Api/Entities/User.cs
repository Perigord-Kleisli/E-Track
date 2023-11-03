using System.ComponentModel.DataAnnotations.Schema;
using ETrack.Models.Dtos;

namespace ETrack.Api.Entities
{


    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = String.Empty;
        public string DisplayName { get; set; } = String.Empty;
        public string PasswordHash { get; set; } = String.Empty;
        public Role Roles {get; set; } = Role.None;
    }
}