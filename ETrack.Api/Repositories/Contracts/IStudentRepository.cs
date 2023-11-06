using ETrack.Models.Dtos;

namespace ETrack.Api.Repositories.Contracts
{
    public interface IStudentRepository
    {
        Task addStudent(Student student);
        Task<Student?> getStudentById(int id);
        Task<IEnumerable<Student>> GetStudents();
    }    
}