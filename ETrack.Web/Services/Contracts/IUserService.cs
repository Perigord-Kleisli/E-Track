using ETrack.Models.Dtos;

namespace ETrack.Web.Services.Contracts
{
    public interface IUserService
    {
        Task<string> UserLogin(UserLoginDto userLoginDto);
        Task UserConfirm(string email);
        Task UserPasswordReset(string email);
        Task UserRegister(UserRegisterDto userRegisterDto);

        Task<IEnumerable<UserDto>> GetUsers();
        Task<UserDto> GetUser(int id);
    }
}