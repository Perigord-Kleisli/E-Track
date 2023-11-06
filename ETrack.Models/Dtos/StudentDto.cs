namespace ETrack.Models.Dtos
{
    public class AddStudentDto
    {
        public int Grade { get; set; }
        public required string Name { get; set; }
    }

    public class SimpleStudentDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Grade { get; set; }
    }


    public class StudentDto
    {
        public int Id { get; set; }
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
        public int Score { get; set; }
        public required string Name { get; set; }
        public int TotalScore { get; set; }
        public SchoolDayDto? Issuance { get; set; }
    }
}