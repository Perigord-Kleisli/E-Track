namespace ETrack.Models.Dtos
{
    public class AddSectionDTO
    {
        public required string Name { get; set; }
        public required int Grade { get; set; }
        public required int AdviserId { get; set; }
    }

    public class SectionDto 
    {
        public required string Name { get; set; }
        public required int Grade { get; set; }
        public required int AdvisorId { get; set; }
        public required IEnumerable<int> StudentIds { get; set; }
    }
}