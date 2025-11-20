using AttendanceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IAttendanceService _attendanceService;

        public StudentController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
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
    }
}
