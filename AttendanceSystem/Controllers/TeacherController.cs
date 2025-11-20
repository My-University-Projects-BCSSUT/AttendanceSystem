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

        public async Task<IActionResult> ClassDetails(int id)
        {
            var classEntity = await _context.Classes
                .Include(c => c.Course)
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

        public async Task<IActionResult> ViewAttendance(int classId)
        {
            var attendances = await _attendanceService.GetClassAttendanceAsync(classId);
            
            var classEntity = await _context.Classes
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.Id == classId);

            ViewBag.Class = classEntity;

            return View(attendances);
        }
    }
}
