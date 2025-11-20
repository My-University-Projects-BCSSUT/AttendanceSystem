namespace AttendanceSystem.Models
{
    public class AttendanceSession
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public DateTime SessionDate { get; set; }
        public string QRCode { get; set; } = string.Empty; // Unique QR code for this session
        public DateTime QRCodeExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Class Class { get; set; } = null!;
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        
        public bool IsQRCodeValid()
        {
            return IsActive && DateTime.UtcNow < QRCodeExpiresAt;
        }
    }
}
