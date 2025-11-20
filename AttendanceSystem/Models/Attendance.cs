namespace AttendanceSystem.Models
{
    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late,
        Excused
    }

    public class Attendance
    {
        public int Id { get; set; }
        public int AttendanceSessionId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;
        public DateTime? CheckInTime { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual AttendanceSession AttendanceSession { get; set; } = null!;
        public virtual ApplicationUser Student { get; set; } = null!;
        
        public bool IsLate(TimeSpan classStartTime, int lateThresholdMinutes = 15)
        {
            if (!CheckInTime.HasValue) return false;
            
            var checkInTimeOnly = CheckInTime.Value.TimeOfDay;
            var lateThreshold = classStartTime.Add(TimeSpan.FromMinutes(lateThresholdMinutes));
            
            return checkInTimeOnly > lateThreshold;
        }
    }
}
