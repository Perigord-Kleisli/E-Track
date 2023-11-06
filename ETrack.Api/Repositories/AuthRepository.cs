using ETrack.Api.Data;
using ETrack.Api.Entities;
using ETrack.Api.Repositories.Contracts;
using ETrack.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ETrack.Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ETrackDBContext etrackDBContext;

        public AuthRepository(ETrackDBContext authDBContext)
        {
            this.etrackDBContext = authDBContext;
        }

        public async Task AddToken(Guid guid, Role role)
        {
            await etrackDBContext.RegisterTokens.AddAsync(new Token {Guid = guid, Role = role});
            await etrackDBContext.SaveChangesAsync();
        }

        // Returns false if there already is a user with the same email
        public async Task<bool> addUser(User user)
        {
            var match = await etrackDBContext
                .Users
                .FirstOrDefaultAsync(x => x.Email == user.Email);

            if (match is not null) return false;

            await etrackDBContext.Users.AddAsync(user);
            await etrackDBContext.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetByUserByEmail(string email)
        {
            return await etrackDBContext
             .Users
             .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<IEnumerable<Student>> getChildren(int userId)
        {
            var user = await etrackDBContext.Users.FindAsync(userId);
            return user!.Children;
        }

        public async Task<Role?> GetToken(Guid guid)
        {
            var guidMatch = await etrackDBContext.RegisterTokens.FirstOrDefaultAsync(x => x.Guid == guid);
            if (guidMatch is null)
            {
                return null;
            }
            else
            {
                etrackDBContext.RegisterTokens.Remove(guidMatch);
                etrackDBContext.SaveChanges();
                return guidMatch.Role;
            }
        }

        public async Task<User?> GetUser(int id)
        {
            return await etrackDBContext.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await etrackDBContext.Users.ToListAsync();
        }
    }
}