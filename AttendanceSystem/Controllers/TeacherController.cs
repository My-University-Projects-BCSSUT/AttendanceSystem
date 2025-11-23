using AttendanceSystem.Data;
using AttendanceSystem.Services;
using AttendanceSystem.Patterns.Singleton;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AttendanceSystem.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAttendanceService _attendanceService;
        private readonly QRCodeService _qrCodeService;

        public TeacherController(ApplicationDbContext context, IAttendanceService attendanceService)
        {
            _context = context;
            _attendanceService = attendanceService;
            _qrCodeService = QRCodeService.Instance;
        }

        public async Task<IActionResult> Index()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();

            return View(classes);
        }

        public async Task<IActionResult> MyClasses()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var classes = await _context.Classes
                .Include(c => c.Course)
                    .ThenInclude(c => c.Semester)
                .Include(c => c.Enrollments)
                .Where(c => c.TeacherId == teacherId)
                .OrderBy(c => c.DayOfWeek)
                .ThenBy(c => c.StartTime)
                .ToListAsync();

            return View(classes);
        }

        public async Task<IActionResult> ClassDetails(int id)
        {
            var classEntity = await _context.Classes
                .Include(c => c.Course)
                    .ThenInclude(co => co.Semester)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classEntity == null)
                return NotFound();

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (classEntity.TeacherId != teacherId)
                return Forbid();

            return View(classEntity);
        }

        [HttpPost]
        public async Task<IActionResult> StartSession(int classId)
        {
            try
            {
                var session = await _attendanceService.CreateSessionAsync(classId, expirationMinutes: 15);
                return RedirectToAction(nameof(ShowQRCode), new { sessionId = session.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(ClassDetails), new { id = classId });
            }
        }

        public async Task<IActionResult> ShowQRCode(int sessionId)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.Class)
                    .ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                return NotFound();

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (session.Class.TeacherId != teacherId)
                return Forbid();

            // Generate QR code data
            var qrData = _qrCodeService.GenerateAttendanceQRData(session.Id, session.QRCode);
            var qrCodeBase64 = _qrCodeService.GenerateQRCodeBase64(qrData);

            ViewBag.QRCodeImage = $"data:image/png;base64,{qrCodeBase64}";
            ViewBag.ExpiresAt = session.QRCodeExpiresAt;

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> CloseSession(int sessionId)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                return NotFound();

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (session.Class.TeacherId != teacherId)
                return Forbid();

            // Mark session as inactive
            await _attendanceService.ExpireSessionAsync(sessionId);
            
            // Mark students who didn't check in as absent
            await _attendanceService.MarkAbsentStudentsAsync(sessionId);

            TempData["SuccessMessage"] = "Session closed successfully. Absent students have been marked.";
            return RedirectToAction(nameof(ViewAttendance), new { classId = session.ClassId });
        }

        public async Task<IActionResult> ViewAttendance(int classId)
        {
            // First, check for expired sessions and mark absent students
            var expiredSessions = await _context.AttendanceSessions
                .Include(s => s.Class)
                    .ThenInclude(c => c.Enrollments)
                .Where(s => s.ClassId == classId && s.IsActive && s.QRCodeExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            foreach (var session in expiredSessions)
            {
                // Mark session as inactive
                await _attendanceService.ExpireSessionAsync(session.Id);
                
                // Mark students who didn't check in as absent
                await _attendanceService.MarkAbsentStudentsAsync(session.Id);
            }

            var attendances = await _attendanceService.GetClassAttendanceAsync(classId);
            
            var classEntity = await _context.Classes
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.Id == classId);

            ViewBag.Class = classEntity;

            return View(attendances);
        }

        public async Task<IActionResult> Reports()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get teacher's classes
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Enrollments)
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();

            // Get all attendance sessions for teacher's classes
            var classIds = classes.Select(c => c.Id).ToList();
            var sessions = await _context.AttendanceSessions
                .Include(s => s.Class)
                .Where(s => classIds.Contains(s.ClassId))
                .ToListAsync();

            // Get all attendance records for teacher's classes
            var attendances = await _context.Attendances
                .Include(a => a.AttendanceSession)
                    .ThenInclude(s => s.Class)
                .Include(a => a.Student)
                .Where(a => classIds.Contains(a.AttendanceSession.ClassId))
                .ToListAsync();

            // Calculate statistics
            var totalClasses = classes.Count;
            var totalSessions = sessions.Count;
            var totalStudents = classes.SelectMany(c => c.Enrollments).Select(e => e.StudentId).Distinct().Count();
            var totalAttendances = attendances.Count;
            var presentCount = attendances.Count(a => a.Status == AttendanceSystem.Models.AttendanceStatus.Present);
            var lateCount = attendances.Count(a => a.Status == AttendanceSystem.Models.AttendanceStatus.Late);
            var absentCount = attendances.Count(a => a.Status == AttendanceSystem.Models.AttendanceStatus.Absent);
            var excusedCount = attendances.Count(a => a.Status == AttendanceSystem.Models.AttendanceStatus.Excused);

            var attendanceRate = totalAttendances > 0 ? (presentCount + lateCount) * 100.0 / totalAttendances : 0;

            ViewBag.Stats = new
            {
                TotalClasses = totalClasses,
                TotalSessions = totalSessions,
                TotalStudents = totalStudents,
                TotalAttendances = totalAttendances,
                PresentCount = presentCount,
                LateCount = lateCount,
                AbsentCount = absentCount,
                ExcusedCount = excusedCount,
                AttendanceRate = Math.Round(attendanceRate, 1)
            };

            ViewBag.RecentAttendances = attendances
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToList();

            return View(classes);
        }
    }
}
