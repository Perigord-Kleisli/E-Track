using ETrack.Models.Dtos;

namespace ETrack.Web.Services.Contracts
{
    public interface IUserService
    {
        Task<string> UserLogin(UserLoginDto userLoginDto);
        Task UserConfirm(string email);
        Task UserRegister(UserRegisterDto userRegisterDto);
    }
}