using System.Runtime.CompilerServices;
using ETrack.Api.Data;
using ETrack.Api.Entities;
using ETrack.Api.Repositories.Contracts;
using ETrack.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ETrack.Api.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IAuthRepository authRepository;
        private readonly ETrackDBContext eTrackDBContext;

        public StudentRepository(IAuthRepository authRepository, ETrackDBContext eTrackDBContext)
        {
            this.authRepository = authRepository;
            this.eTrackDBContext = eTrackDBContext;
        }

        public async Task addSectionAsync(Section section)
        {
            await eTrackDBContext.Sections.AddAsync(section);
            await eTrackDBContext.SaveChangesAsync();
        }

        public async Task addStudentAsync(Student student, int sectionId)
        {
            await eTrackDBContext.Students.AddAsync(student);
            await eTrackDBContext.SaveChangesAsync();
        }

        public Section GetSection(int id)
        {
            return GetSections().ToList().First(x => x.Id == id);
        }

        public IEnumerable<Section> GetSections()
        {
            var ids = 
                eTrackDBContext.Database
                    .SqlQuery<int>( $"SELECT Id FROM Sections")
                    .ToList();
            var names = 
                eTrackDBContext.Database
                    .SqlQuery<string>( $"SELECT Name FROM Sections")
                    .ToList();
            var grades = 
                eTrackDBContext.Database
                    .SqlQuery<int>( $"SELECT Grade FROM Sections")
                    .ToList();
            var adviserIds = 
                eTrackDBContext.Database
                    .SqlQuery<int>( $"SELECT AdviserId FROM Sections")
                    .ToList();
            var studentIds = ids.Select(id =>
                eTrackDBContext.Database
                    .SqlQuery<int>($@"
                        SELECT Id FROM Students
                        WHERE SectionId = {id}
                    ")
                    .ToList());
            //This is not an optimal solution
            return 
                from id in ids
                from name in names
                from grade in grades
                from adviser in adviserIds.Select(authRepository.GetUser)
                from students in studentIds
                select new Section {
                    Id = id,
                    Name = name,
                    Grade = grade,
                    Adviser = adviser,
                    Students = students.Select(getStudentById).ToList(),
                    Schedule = new List<Schedule>{}
                };
        }

        public Student getStudentById(int id)
        {
            return eTrackDBContext.Students.Find(id)!;
        }

        public async Task<Student?> getStudentByIdAsync(int id)
        {
            return await eTrackDBContext.Students.FindAsync(id);
        }

        public IEnumerable<Student> GetStudents()
        {
            return eTrackDBContext.Students.ToList();
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync()
        {
            return await eTrackDBContext.Students.ToListAsync();
        }
    }
}