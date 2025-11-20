namespace AttendanceSystem.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., "Tutorial 1", "Lecture A"
        public int CourseId { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Course Course { get; set; } = null!;
        public virtual ApplicationUser Teacher { get; set; } = null!;
        public virtual ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
        public virtual ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
        
        public string GetScheduleInfo()
        {
            return $"{DayOfWeek} {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
        }
    }
}
