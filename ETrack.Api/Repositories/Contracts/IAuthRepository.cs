using ETrack.Api.Entities;
using ETrack.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ETrack.Api.Repositories.Contracts
{
    public interface IAuthRepository
    {
       Task<User?> GetUserAsync(int id);
       Task<IEnumerable<User>> GetUsers();
       Task<User?> GetByUserByEmailAsync(string usernam);

       Task<bool> addUserAsync(User user);
       Task<IEnumerable<Student>> getChildrenAsync(int userId);
       Task<Role> GetTokenAsync(string uid);
       Task<UserRegisterToken> GenTokenAsync(Role role);
       Task<Guid> CreateConfirmationTokenAsync(User unconfirmedUser);
       Task UseConfirmationTokenAsync(Guid id);

       Task<Guid> CreatePasswordForgotTokenAsync(User user);
       Task UsePasswordForgotTokenAsync(Guid id, string password);
    }
}