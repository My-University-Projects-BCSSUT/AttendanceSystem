namespace AttendanceSystem.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // e.g., "COS20007"
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int SemesterId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Semester Semester { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
        
        public string GetFullCourseName()
        {
            return $"{Code} - {Name}";
        }
    }
}
