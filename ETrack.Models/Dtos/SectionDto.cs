namespace ETrack.Models.Dtos
{
    public class AddSectionDTO
    {
        public required string Name { get; set; }

        [System.ComponentModel.DataAnnotations.Range(1,12, ErrorMessage = "Must be between grades 1-12")]
        public required int Grade { get; set; }
        public required int AdviserId { get; set; }
    }

    public class SectionDto 
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        [System.ComponentModel.DataAnnotations.Range(1,12, ErrorMessage = "Must be between grades 1-12")]
        public required int Grade { get; set; }
        public required int AdvisorId { get; set; }
        public required IEnumerable<int> StudentIds { get; set; }
    }
}