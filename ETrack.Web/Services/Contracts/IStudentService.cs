using ETrack.Models.Dtos;

namespace ETrack.Web.Services.Contracts
{
    public interface IStudentService
    {
        Task CreateSection(AddSectionDTO addSectionDTO);
        Task<IEnumerable<SectionDto>> GetSections();
        Task<SectionDto> GetSection(int id);

        Task CreateStudent(SimpleStudentDto simpleStudentDto);
        Task<IEnumerable<SimpleStudentDto>> GetStudents();
    }
}