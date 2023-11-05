using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ETrack.Api.Entities;

namespace ETrack.Api
{
    public class Subject
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required User Teacher { get; set; }
        public required int GradeLevel { get; set; }
        public List<Activity> Activities { get; set; } = new List<Activity>();
    }

    public class Activity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int TotalScore { get; set; }

        [Key]
        public int IssuanceId { get; set; }
        [ForeignKey("IssuanceId")]
        public SchoolDay? Issuance { get; set; }
    }
}
