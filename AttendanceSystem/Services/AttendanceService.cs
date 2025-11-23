using AttendanceSystem.Data;
using AttendanceSystem.Models;
using AttendanceSystem.Patterns.Builder;
using AttendanceSystem.Patterns.Singleton;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Services
{
    public interface IAttendanceService
    {
        Task<AttendanceSession> CreateSessionAsync(int classId, int expirationMinutes = 15);
        Task<Attendance> SubmitAttendanceAsync(int sessionId, string studentId, string qrCode);
        Task<List<Attendance>> GetClassAttendanceAsync(int classId);
        Task<AttendanceSession?> GetActiveSessionByQRCodeAsync(string qrCode);
        Task ExpireSessionAsync(int sessionId);
        Task MarkAbsentStudentsAsync(int sessionId);
    }

    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAttendanceSessionBuilder _sessionBuilder;
        private readonly QRCodeService _qrCodeService;

        public AttendanceService(ApplicationDbContext context, IAttendanceSessionBuilder sessionBuilder)
        {
            _context = context;
            _sessionBuilder = sessionBuilder;
            _qrCodeService = QRCodeService.Instance;
        }

        public async Task<AttendanceSession> CreateSessionAsync(int classId, int expirationMinutes = 15)
        {
            var classEntity = await _context.Classes
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classEntity == null)
                throw new InvalidOperationException("Class not found.");

            // Use Builder Pattern to create session
            var session = _sessionBuilder
                .SetClass(classEntity)
                .SetSessionDate(DateTime.UtcNow)
                .SetQRCodeExpiration(expirationMinutes)
                .SetActive(true)
                .Build();

            _context.AttendanceSessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<Attendance> SubmitAttendanceAsync(int sessionId, string studentId, string qrCode)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null || session.QRCode != qrCode)
                throw new InvalidOperationException("Invalid session or QR code.");

            if (!session.IsQRCodeValid())
                throw new InvalidOperationException("QR code has expired.");

            // Check if student is enrolled
            var enrollment = await _context.ClassEnrollments
                .AnyAsync(e => e.ClassId == session.ClassId && e.StudentId == studentId);

            if (!enrollment)
                throw new InvalidOperationException("Student is not enrolled in this class.");

            // Check if attendance already submitted
            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.AttendanceSessionId == sessionId && a.StudentId == studentId);

            if (existingAttendance != null)
                throw new InvalidOperationException("Attendance already submitted for this session.");

            // Create attendance record
            var checkInTime = DateTime.UtcNow;
            var isLate = checkInTime.TimeOfDay > session.Class.StartTime.Add(TimeSpan.FromMinutes(15));

            var attendance = new Attendance
            {
                AttendanceSessionId = sessionId,
                StudentId = studentId,
                CheckInTime = checkInTime,
                Status = isLate ? AttendanceStatus.Late : AttendanceStatus.Present,
                CreatedAt = DateTime.UtcNow
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return attendance;
        }

        public async Task<List<Attendance>> GetClassAttendanceAsync(int classId)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.AttendanceSession)
                .Where(a => a.AttendanceSession.ClassId == classId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<AttendanceSession?> GetActiveSessionByQRCodeAsync(string qrCode)
        {
            return await _context.AttendanceSessions
                .Include(s => s.Class)
                    .ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(s => s.QRCode == qrCode && s.IsActive);
        }

        public async Task ExpireSessionAsync(int sessionId)
        {
            var session = await _context.AttendanceSessions.FindAsync(sessionId);
            if (session != null && session.IsActive)
            {
                session.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAbsentStudentsAsync(int sessionId)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.Class)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null) return;

            // Get all enrolled students for this class
            var enrolledStudentIds = session.Class.Enrollments.Select(e => e.StudentId).ToList();

            // Get students who already have attendance records
            var studentsWithAttendance = await _context.Attendances
                .Where(a => a.AttendanceSessionId == sessionId)
                .Select(a => a.StudentId)
                .ToListAsync();

            // Find students who are enrolled but didn't check in
            var absentStudentIds = enrolledStudentIds.Except(studentsWithAttendance).ToList();

            // Create absent records for these students
            foreach (var studentId in absentStudentIds)
            {
                var attendance = new Attendance
                {
                    AttendanceSessionId = sessionId,
                    StudentId = studentId,
                    CheckInTime = null,
                    Status = AttendanceStatus.Absent,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Attendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
        }
    }
}
