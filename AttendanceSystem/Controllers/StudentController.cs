using AttendanceSystem.Services;
using AttendanceSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AttendanceSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ApplicationDbContext _context;

        public StudentController(IAttendanceService attendanceService, ApplicationDbContext context)
        {
            _attendanceService = attendanceService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ScanQR()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAttendance(string qrCode)
        {
            try
            {
                var session = await _attendanceService.GetActiveSessionByQRCodeAsync(qrCode);
                
                if (session == null || !session.IsQRCodeValid())
                {
                    TempData["ErrorMessage"] = "Invalid or expired QR code.";
                    return RedirectToAction(nameof(ScanQR));
                }

                var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(studentId))
                {
                    TempData["ErrorMessage"] = "Unable to identify student.";
                    return RedirectToAction(nameof(ScanQR));
                }

                var attendance = await _attendanceService.SubmitAttendanceAsync(session.Id, studentId, qrCode);

                TempData["SuccessMessage"] = $"Attendance recorded successfully for {session.Class.Course.Name}!";
                return RedirectToAction(nameof(AttendanceSuccess));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(ScanQR));
            }
        }

        public IActionResult AttendanceSuccess()
        {
            return View();
        }

        public async Task<IActionResult> MyAttendance()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get student's enrollments
            var enrollments = await _context.ClassEnrollments
                .Include(e => e.Class)
                    .ThenInclude(c => c.Course)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();

            // Get student's attendance records
            var attendances = await _context.Attendances
                .Include(a => a.AttendanceSession)
                    .ThenInclude(s => s.Class)
                        .ThenInclude(c => c.Course)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            // Calculate statistics
            var totalClasses = enrollments.Count;
            var totalSessions = attendances.Count;
            var presentCount = attendances.Count(a => a.Status == AttendanceSystem.Models.AttendanceStatus.Present);
            var lateCount = attendances.Count(a => a.Status == AttendanceSystem.Models.AttendanceStatus.Late);
            var absentCount = attendances.Count(a => a.Status == AttendanceSystem.Models.AttendanceStatus.Absent);
            var excusedCount = attendances.Count(a => a.Status == AttendanceSystem.Models.AttendanceStatus.Excused);

            var attendanceRate = totalSessions > 0 ? (presentCount + lateCount) * 100.0 / totalSessions : 0;

            ViewBag.Stats = new
            {
                TotalClasses = totalClasses,
                TotalSessions = totalSessions,
                PresentCount = presentCount,
                LateCount = lateCount,
                AbsentCount = absentCount,
                ExcusedCount = excusedCount,
                AttendanceRate = Math.Round(attendanceRate, 1)
            };

            return View(attendances);
        }

        public async Task<IActionResult> MyClasses()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var enrollments = await _context.ClassEnrollments
                .Include(e => e.Class)
                    .ThenInclude(c => c.Course)
                        .ThenInclude(co => co.Semester)
                .Include(e => e.Class)
                    .ThenInclude(c => c.Teacher)
                .Where(e => e.StudentId == studentId)
                .OrderBy(e => e.Class.DayOfWeek)
                .ThenBy(e => e.Class.StartTime)
                .ToListAsync();

            return View(enrollments);
        }
    }
}
