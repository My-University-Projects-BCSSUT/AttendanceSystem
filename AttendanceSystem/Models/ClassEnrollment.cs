namespace AttendanceSystem.Models
{
    public class ClassEnrollment
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Class Class { get; set; } = null!;
        public virtual ApplicationUser Student { get; set; } = null!;
    }
}
