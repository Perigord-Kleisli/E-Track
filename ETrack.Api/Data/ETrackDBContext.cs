using ETrack.Api.Entities;
using ETrack.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ETrack.Api.Data
{
    public class ETrackDBContext : DbContext
    {
        public ETrackDBContext(DbContextOptions<ETrackDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SchoolDay> SchoolDays { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Token> RegisterTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //REMOVE THIS IF IT BECOMES AN ENTERPRISE PRODUCT
            //THIS BLOCK IS PURELY FOR DEVELOPMENT PURPOSES

            //TEAM HASKELL IS THE NAME OF A 1ST YEAR GROUP
            //WE ARE NOT AFFILIATED WITH THE HASKELL FOUNDATION

            //The following are test data for testing

            // var schoolDay1 = new SchoolDay {Id = 3, Date = DateTime.Now};
            // var schoolDay2 = new SchoolDay {Id = 4, Date = DateTime.MinValue};
            // var schoolDay3 = new SchoolDay {Id = 5, Date = DateTime.MaxValue};
            // var schoolDay4 = new SchoolDay {Id = 6, Date = DateTime.Parse("04/23/02")};

            // modelBuilder.Entity<SchoolDay>().HasData(schoolDay1);
            // modelBuilder.Entity<SchoolDay>().HasData(schoolDay2);
            // modelBuilder.Entity<SchoolDay>().HasData(schoolDay3);
            // modelBuilder.Entity<SchoolDay>().HasData(schoolDay4);

            // var activities = new List<Activity> {
            //     new Activity {
            //         Id = 1, Name = "Functors", TotalScore = 20, IssuanceId = schoolDay1.Id
            //     },
            //     new Activity {
            //         Id = 2, Name = "Applicatives", TotalScore = 30, IssuanceId = schoolDay2.Id
            //     },
            //     new Activity {
            //         Id = 3, Name = "Monads", TotalScore = 40, IssuanceId = schoolDay3.Id
            //     },
            // };
            // foreach (var act in activities)
            //     modelBuilder.Entity<Activity>().HasData(act);

            // var students = new List<Student> {
            //     new Student {
            //         Id = 1,
            //         Name = "Literal Newborn Baby",
            //         Attendance = new List<SchoolDay>{schoolDay1, schoolDay2, schoolDay3, schoolDay4},
            //     },
            // };
            // foreach (var student in students)
            //     modelBuilder.Entity<Student>().HasData(student);
            //     new Student {
            //         Id = 2,
            //         Name = "The one from the beginning",
            //         Attendance = schoolDays,
            //         Scores = new List<CompletedActivity>{
            //             CompletedActivity.ScoreActivity(activities[0], 20),
            //             CompletedActivity.ScoreActivity(activities[1], 30),
            //             CompletedActivity.ScoreActivity(activities[2], 40),
            //         }
            //     },
            //     new Student {
            //         Id = 3,
            //         Name = "Billy Fergurson",
            //         Attendance = schoolDays,
            //         Scores = new List<CompletedActivity>{
            //             CompletedActivity.ScoreActivity(activities[0], 10),
            //             CompletedActivity.ScoreActivity(activities[1], 20),
            //             CompletedActivity.ScoreActivity(activities[2], 10),
            //         }
            //     },
            //     new Student {
            //         Id = 4,
            //         Name = "Mitchellin Albastar",
            //         Attendance = schoolDays,
            //         Scores = new List<CompletedActivity>{
            //             CompletedActivity.ScoreActivity(activities[0], 12),
            //             CompletedActivity.ScoreActivity(activities[1], 23),
            //             CompletedActivity.ScoreActivity(activities[2], 11),
            //         }
            //     }
            // };
            // modelBuilder.Entity<Student>().HasData(students);

            // var userStudentAllocation = new UserStudentAllocation {
            //         Id = 1, Students = students
            //     };

            // modelBuilder.Entity<UserStudentAllocation>().HasData(userStudentAllocation);

            // var superuser = new User {
            //     Id = 1,
            //     Email = "teamhaskelladminacct@gmail.com",
            //     FullName = "Team Haskell",
            //     FirstName = "Team",
            //     LastName = "Haskell",
            //     PasswordHash= "$2a$11$is4ITdpiSVvceEBucBkPLeIvMLVy/C2mcrSdPKotgAT8Bh5n9LW0G",
            //     Roles = Role.Parent | Role.Teacher | Role.Admin,
            //     IsEmailConfirmed = false,
            //     CreationDate = DateTime.Now,
            //     BirthDate = DateTime.Parse("02/16/2005"),
            //     // UserStudentAllocation = userStudentAllocation
            // };

            // modelBuilder.Entity<User>().HasData(superuser);
            // modelBuilder.Entity<Subject>().HasData(new Subject {
            //     Id = 1,
            //     Teacher = superuser,
            //     Name = "Haskell",
            //     GradeLevel = 12,
            //     Activities = activities
            // });
        }
    }
}