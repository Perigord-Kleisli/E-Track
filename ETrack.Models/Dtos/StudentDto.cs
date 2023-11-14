namespace ETrack.Models.Dtos
{
    public class SimpleStudentDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        [System.ComponentModel.DataAnnotations.Range(1,12, ErrorMessage = "Must be between grades 1-12")]
        public int Grade { get; set; }
        public int SectionId { get; set; }
    }


    public class StudentDto
    {
        public int Id { get; set; }
        [System.ComponentModel.DataAnnotations.Range(1,12, ErrorMessage = "Must be between grades 1-12")]
        public required int Grade { get; set; }
        public required string Name { get; set; }
        public List<SchoolDayDto> Attendance { get; set; } = new List<SchoolDayDto>();
        public List<CompletedActivityDto> Scores { get; set; } = new List<CompletedActivityDto>();
    }

    public class SchoolDayDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
 
    public class CompletedActivityDto 
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Score { get; set; }
        public required int TotalScore { get; set; }
        public required SchoolDayDto Issuance { get; set; }
        public required DateTime? Deadline { get; set; }
    }
}