using Microsoft.AspNetCore.Identity;

namespace AttendanceSystem.Models
{
    // Base User class using Identity
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        
        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
