using System.Formats.Asn1;
using ETrack.Api.Data;
using ETrack.Api.Entities;
using ETrack.Api.Repositories.Contracts;
using ETrack.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ETrack.Api.Repositories
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
            return true;       // [HttpPost("generate-forgot-password-token")]
        // public async Task;
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

        public async Task<UserRegisterToken> GenToken(Role roles)
        {
            var rand = new Random();
            var uid = (rand.Next() * 46656).ToString("X8");

            while (await etrackDBContext.RegisterTokens.AnyAsync(x => x.Uid == uid ))
            {
                uid = (rand.Next() * 46656).ToString("X8");
            }

            var token = new UserRegisterToken
            {
                Uid = uid,
                CreationDate = DateTime.Now,
                Role = roles
            };
            await etrackDBContext.RegisterTokens.AddAsync(token);
            await etrackDBContext.SaveChangesAsync();
            return token;
        }

        public async Task<Role> GetToken(string uid)
        {
            var guidMatch = await etrackDBContext
                .RegisterTokens
                .FirstOrDefaultAsync(x => x.Uid.ToUpper() == uid.ToUpper());
            if (guidMatch is null)
            {
                throw new Exception("Registration GUID not found in database");
            }

            if (DateTime.Now > guidMatch.CreationDate.AddHours(6))
            {
                throw new Exception("Registration GUID is expired");
            }
            etrackDBContext.RegisterTokens.Remove(guidMatch);
            await etrackDBContext.SaveChangesAsync();
            return guidMatch.Role;
        }

        public async Task<Guid> CreateConfirmationToken(User unconfirmedUser)
        {
            var guid = Guid.NewGuid();
            var token = new UserEmailConfirmationToken {
                UserId = unconfirmedUser.Id,
                Guid = guid,
                CreationDate = DateTime.Now
            };
            await etrackDBContext.EmailConfirmationTokens.AddAsync(token);
            await etrackDBContext.SaveChangesAsync();
            return guid;
        }

        public async Task<User?> GetUser(int id)
        {
            return await etrackDBContext.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await etrackDBContext.Users.ToListAsync();
        }

        public async Task UseConfirmationToken(Guid guid)
        {
            var guidMatch = await etrackDBContext
                .EmailConfirmationTokens
                .FirstOrDefaultAsync(x => x.Guid == guid);
            if (guidMatch is null)
            {
                throw new Exception("GUID not found in database");
            }

            if (DateTime.Now > guidMatch.CreationDate.AddHours(1))
            {
                throw new Exception("Confirmation GUID is expired");
            }

            var user = await etrackDBContext.Users.FindAsync(guidMatch.UserId);

            if (user is null) 
            {
                throw new Exception("User not found");
            }

            if (user.IsEmailConfirmed)
            {
                throw new Exception("User already has a confirmed email");
            }

            user.IsEmailConfirmed = true;
            etrackDBContext.EmailConfirmationTokens.Remove(guidMatch);
            await etrackDBContext.SaveChangesAsync();
        }

        public async Task<Guid> GeneratePasswordForgotToken(User user)
        {
            var guid = Guid.NewGuid();
            var token = new UserPasswordForgotToken {
                UserId = user.Id,
                Guid = guid,
                CreationDate = DateTime.Now
            };
            await etrackDBContext.PasswordForgotTokens.AddAsync(token);
            await etrackDBContext.SaveChangesAsync();
            return guid;
        }

        public async Task UsePasswordForgotToken(Guid guid)
        {
            var guidMatch = await etrackDBContext
                .PasswordForgotTokens
                .FirstOrDefaultAsync(x => x.Guid == guid);
            if (guidMatch is null)
            {
                throw new Exception("GUID not found in database");
            }

            if (DateTime.Now > guidMatch.CreationDate.AddHours(1))
            {
                throw new Exception("Confirmation GUID is expired");
            }

            var user = await etrackDBContext.Users.FindAsync(guidMatch.UserId);
            user!.IsEmailConfirmed = true;
            etrackDBContext.PasswordForgotTokens.Remove(guidMatch);
            await etrackDBContext.SaveChangesAsync();
        }

        public Task<Guid> CreatePasswordForgotToken(User user)
        {
            throw new NotImplementedException();
        }
    }
}