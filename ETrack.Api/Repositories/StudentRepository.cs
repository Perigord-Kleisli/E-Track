using ETrack.Api.Data;
using ETrack.Api.Repositories.Contracts;
using ETrack.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ETrack.Api.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ETrackDBContext eTrackDBContext;

        public StudentRepository(ETrackDBContext eTrackDBContext)
        {
            this.eTrackDBContext = eTrackDBContext;
        }
        public async Task addStudent(Student student)
        {
            await eTrackDBContext.Students.AddAsync(student);
            await eTrackDBContext.SaveChangesAsync();
        }

        public async Task<Student?> getStudentById(int id)
        {
            return await eTrackDBContext.Students.FindAsync(id);
        }

        public async Task<IEnumerable<Student>> GetStudents()
        {
            return await eTrackDBContext.Students.ToListAsync();
        }
    }
}