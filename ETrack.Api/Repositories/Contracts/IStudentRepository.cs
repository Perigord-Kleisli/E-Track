using ETrack.Api.Entities;
using ETrack.Models.Dtos;

namespace ETrack.Api.Repositories.Contracts
{
    public interface IStudentRepository
    {
        Task addStudentAsync(Student student);
        Task<Student?> getStudentByIdAsync(int id);
        Task<IEnumerable<Student>> GetStudentsAsync();
        Task addSectionAsync(Section section);
        Task<IEnumerable<Section>> GetSectionsAsync();
    }    
}