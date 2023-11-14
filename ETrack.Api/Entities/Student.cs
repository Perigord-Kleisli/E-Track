using System.ComponentModel.DataAnnotations.Schema;

namespace ETrack.Api.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public required int Grade { get; set; }
        public required string Name { get; set; }
        public List<SchoolDay> Attendance { get; set; } = new List<SchoolDay>();
        public List<CompletedActivity> Scores { get; set; } = new List<CompletedActivity>();
        [ForeignKey("Section")]
        public int SectionId { get; set; }
        public Section Section { get; set; } = default!;

    }

    public class CompletedActivity : Activity
    {
        public int Score { get; set; }
    }

    public class IsChildRequest 
    {
        public int Id { get; set; }
        public required DateTime CreationDate { get; set; }
        public required User Parent { get; set; }
        public required Student Student { get; set; }

    }
}