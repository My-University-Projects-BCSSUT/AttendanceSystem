namespace AttendanceSystem.Models
{
    public class Semester
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., "Semester 1 2025"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
        
        public bool IsCurrentSemester()
        {
            var now = DateTime.UtcNow;
            return now >= StartDate && now <= EndDate && IsActive;
        }
    }
}
