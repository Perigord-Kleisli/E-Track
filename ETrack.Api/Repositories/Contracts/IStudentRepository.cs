using ETrack.Api.Entities;
using ETrack.Models.Dtos;

namespace ETrack.Api.Repositories.Contracts
{
    public interface IStudentRepository
    {
        Task addStudentAsync(Student student, int sectionId);
        Task<Student?> getStudentByIdAsync(int id);
        Student getStudentById(int id);
        Task<IEnumerable<Student>> GetStudentsAsync();
        IEnumerable<Student> GetStudents();
        Task addSectionAsync(Section section);
        IEnumerable<Section> GetSections();
        Section GetSection(int Id);
    }    
}