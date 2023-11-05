using ETrack.Models.Dtos;

namespace ETrack.Web.Services.Contracts
{
    public interface IUserService
    {
        Task<string> UserLogin(UserLoginDto userLoginDto);
        Task UserRegister(UserRegisterDto userRegisterDto);
    }
}