using ETrack.Models.Dtos;
using ETrack.Web.Services.Contracts;

namespace ETrack.Web.Services
{
    public class StudentService : IStudentService
    {
        private readonly HttpClient httpClient;

        public StudentService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task CreateSection(AddSectionDTO addSectionDTO)
        {
            try 
            {
                var response = await httpClient.PostAsJsonAsync("/api/Students/sections", addSectionDTO);
                if (!response.IsSuccessStatusCode) 
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<SectionDto>> GetSections()
        {
            try 
            {
                var response = await httpClient.GetAsync("/api/Students/sections");
                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
                return (await response.Content.ReadFromJsonAsync<IEnumerable<SectionDto>>())!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}