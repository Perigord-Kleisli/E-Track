using ETrack.Api.Entities;
using ETrack.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ETrack.Api.Repositories.Contracts
{
    public interface IAuthRepository
    {
       Task<User?> GetUser(int id);
       Task<IEnumerable<User>> GetUsers();
       Task<User?> GetByUserByEmail(string usernam);

       Task<bool> addUser(User user);
       Task<IEnumerable<Student>> getChildren(int userId);
       Task AddToken(Guid guid, Role role);
       Task<Role?> GetToken(Guid guid);
    }
}