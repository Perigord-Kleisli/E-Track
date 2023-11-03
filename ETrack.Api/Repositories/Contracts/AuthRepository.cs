using ETrack.Api.Data;
using ETrack.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETrack.Api.Repositories.Contracts
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDBContext authDBContext;

        public AuthRepository(AuthDBContext authDBContext)
        {
            this.authDBContext = authDBContext;
        }

        // Returns false if there already is a user with the same email
        public async Task<bool> addUser(User user)
        {
            var match = await authDBContext
                .Users
                .FirstOrDefaultAsync(x => x.Email == user.Email);

            if (match is not null) return false;

            await authDBContext.Users.AddAsync(user);
            await authDBContext.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetByUserByName(string email)
        {
            return await authDBContext
             .Users
             .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<User?> GetUser(int id)
        {
            return await authDBContext.Users.FindAsync(id);
        }
    }
}