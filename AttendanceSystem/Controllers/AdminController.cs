using AttendanceSystem.Data;
using AttendanceSystem.Models;
using AttendanceSystem.Patterns.Builder;
using AttendanceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICourseBuilder _courseBuilder;
        private readonly IStudentService _studentService;

        public AdminController(ApplicationDbContext context, ICourseBuilder courseBuilder, IStudentService studentService)
        {
            _context = context;
            _courseBuilder = courseBuilder;
            _studentService = studentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Semester Management
        public async Task<IActionResult> Semesters()
        {
            var semesters = await _context.Semesters.OrderByDescending(s => s.StartDate).ToListAsync();
            return View(semesters);
        }

        [HttpGet]
        public IActionResult CreateSemester()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSemester(Semester semester)
        {
            if (ModelState.IsValid)
            {
                _context.Semesters.Add(semester);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Semesters));
            }
            return View(semester);
        }

        // Course Management
        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses
                .Include(c => c.Semester)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCourse()
        {
            ViewBag.Semesters = new SelectList(await _context.Semesters.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(string code, string name, string description, int credits, int semesterId)
        {
            try
            {
                var semester = await _context.Semesters.FindAsync(semesterId);
                if (semester == null)
                {
                    ModelState.AddModelError("", "Invalid semester selected.");
                    ViewBag.Semesters = new SelectList(await _context.Semesters.ToListAsync(), "Id", "Name");
                    return View();
                }

                // Use Builder Pattern
                var course = _courseBuilder
                    .SetCode(code)
                    .SetName(name)
                    .SetDescription(description)
                    .SetCredits(credits)
                    .SetSemester(semester)
                    .Build();

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Courses));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Semesters = new SelectList(await _context.Semesters.ToListAsync(), "Id", "Name");
                return View();
            }
        }

        // Class Management
        public async Task<IActionResult> Classes()
        {
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(classes);
        }

        // Student Management
        public async Task<IActionResult> Students()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        [HttpGet]
        public IActionResult ImportStudents()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportStudents(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a valid Excel file.");
                return View();
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var importedStudents = await _studentService.ImportStudentsFromExcelAsync(stream);
                    TempData["SuccessMessage"] = $"Successfully imported {importedStudents.Count} students.";
                }
                return RedirectToAction(nameof(Students));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error importing students: {ex.Message}");
                return View();
            }
        }
    }
}
