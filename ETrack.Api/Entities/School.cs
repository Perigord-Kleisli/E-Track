using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ETrack.Api.Entities;

namespace ETrack.Api.Entities
{
    public class Section
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Grade { get; set; }
        public required User Adviser { get; set; }
        public required List<Student> Students { get; set; }
        // public required List<Schedule> Schedule { get; set; }
    }

    public enum Day
    {
        Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
    }

    public class Schedule
    {
       public int Id { get; set; } 
       public required Day Day { get; set; }
       public required List<SubjectSchedule> SubjectSchedules { get; set; }
    }

    public class SubjectSchedule
    {
        public int Id { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public Subject? Subject { get; set; }
    }

    public class SchoolYear 
    {
        public int Id { get; set; }
        public required int YearStart { get; set; }
        public required int YearEnd { get; set; }
        public int DaysPassed { get; set; }
        public required List<SchoolDay> Days { get; set; }
    }

    public class SchoolDay 
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
}