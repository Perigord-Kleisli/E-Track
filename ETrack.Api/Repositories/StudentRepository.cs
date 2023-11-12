using ETrack.Api.Data;
using ETrack.Api.Entities;
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

        public async Task addSectionAsync(Section section)
        {
            await eTrackDBContext.Sections.AddAsync(section);
            await eTrackDBContext.SaveChangesAsync();
        }

        public async Task addStudentAsync(Student student)
        {
            await eTrackDBContext.Students.AddAsync(student);
            await eTrackDBContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Section>> GetSectionsAsync()
        {
            return await eTrackDBContext.Sections.ToListAsync();
        }

        public async Task<Student?> getStudentByIdAsync(int id)
        {
            return await eTrackDBContext.Students.FindAsync(id);
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync()
        {
            return await eTrackDBContext.Students.ToListAsync();
        }
    }
}