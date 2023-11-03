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
       public required string Email { get; set; }
       public required string Password { get; set; }
    }

    public class UserRegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string DisplayName { get; set; }
        public required Role Permission { get; set; }
    }
}