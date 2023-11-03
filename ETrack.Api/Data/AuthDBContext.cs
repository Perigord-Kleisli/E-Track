using ETrack.Api.Entities;
using ETrack.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ETrack.Api.Data
{
    public class AuthDBContext : DbContext
    {
        public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //REMOVE THIS IF IT BECOMES AN ENTERPRISE PRODUCT
            //THIS BLOCK IS PURELY FOR DEVELOPMENT PURPOSES

            //TEAM HASKELL IS THE NAME OF A 1ST YEAR GROUP
            //WE ARE NOT AFFILIATED WITH THE HASKELL FOUNDATION
            modelBuilder.Entity<User>().HasData(new User {
                Id = 1,
                Email = "teamhaskelladminacct@gmail.com",
                DisplayName= "teamhaskelladminacct@gmail.com",
                //Hash of: teamhaskellpasswd
                PasswordHash= "$2a$11$is4ITdpiSVvceEBucBkPLeIvMLVy/C2mcrSdPKotgAT8Bh5n9LW0G",
                Roles = Role.Parent | Role.Teacher | Role.Admin
            });
        }
    }
}