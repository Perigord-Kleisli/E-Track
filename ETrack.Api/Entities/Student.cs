namespace ETrack.Api
{
    public class Student
    {
        public int Id { get; set; }
        public required int Grade { get; set; }
        public required string Name { get; set; }
        public List<SchoolDay> Attendance { get; set; } = new List<SchoolDay>();
        public List<CompletedActivity> Scores { get; set; } = new List<CompletedActivity>();
    }

    public class SchoolDay 
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
 
    public class CompletedActivity : Activity
    {
        public int Score { get; set; }

        public static CompletedActivity ScoreActivity(Activity activity, int score) {
            return new CompletedActivity {
                Id = activity.Id,
                Name = activity.Name,
                TotalScore = activity.TotalScore,
                Score = score,
                Issuance = activity.Issuance,
            };
        }

    }
}