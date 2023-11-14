using ETrack.Api.Entities;
using ETrack.Models.Dtos;

namespace ETrack.Api.Extensions
{
    public static class DtoConversions
    {
        public static StudentDto ConvertToDto(this Student student)
            => new StudentDto
            {
                Id = student.Id,
                Grade = student.Grade,
                Name = student.Name,
                Attendance = student
                    .Attendance
                    .Select(ConvertToDto)
                    .ToList(),
                Scores = student
                    .Scores
                    .Select(ConvertToDto)
                    .ToList(),
                
            };

        public static SchoolDayDto ConvertToDto(this SchoolDay schoolDay)
            => new SchoolDayDto
            {
                Id = schoolDay.Id,
                Date = schoolDay.Date
            };

        public static CompletedActivityDto ConvertToDto(this CompletedActivity completedActivity)
            => new CompletedActivityDto
            {
                Id = completedActivity.Id,
                Score = completedActivity.Score,
                TotalScore = completedActivity.TotalScore,
                Name = completedActivity.Name,
                Issuance = completedActivity.Issuance.ConvertToDto(),
                Deadline = completedActivity.Deadline
            };

        public static UserDto ConvertToDto(this User user) 
            => new UserDto 
            {
                Id = user.Id,
                FullName = user.FullName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.Roles,
                BirthDate = user.BirthDate,
            };

        public static SectionDto ConvertToDto(this Section section)
            => new SectionDto 
            {
                Id = section.Id,
                Name = section.Name,
                Grade = section.Grade,
                AdvisorId = section.Adviser.Id,
                StudentIds = section.Students.Select(x => x.Id)
            };
    }

    public static class FuncUtils
    {
        public static R? Fmap<T, R>(this T? x, Func<T, R> f)
            where T : struct
            where R : struct
        {
            if (x is not null)
            {
                return f(x.Value);
            }
            else
            {
                return null;
            }
        }

    }
}