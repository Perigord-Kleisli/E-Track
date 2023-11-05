using ETrack.Api.Data;
using ETrack.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETrack.Api.Repositories.Contracts
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ETrackDBContext etrackDBContext;

        public AuthRepository(ETrackDBContext authDBContext)
        {
            this.etrackDBContext = authDBContext;
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

        public async Task<User?> GetByUserByName(string email)
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

        public async Task<User?> GetUser(int id)
        {
            return await etrackDBContext.Users.FindAsync(id);
        }
    }
}