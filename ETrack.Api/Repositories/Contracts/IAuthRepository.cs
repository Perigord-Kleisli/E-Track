using ETrack.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ETrack.Api.Repositories.Contracts
{
    public interface IAuthRepository
    {
       Task<User?> GetUser(int id);
       Task<User?> GetByUserByName(string usernam);

       Task<bool> addUser(User user);
    }
}