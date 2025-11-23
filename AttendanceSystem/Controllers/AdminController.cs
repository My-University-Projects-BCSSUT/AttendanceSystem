using AttendanceSystem.Data;
using AttendanceSystem.Models;
using AttendanceSystem.Patterns.Builder;
using AttendanceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
                semester.CreatedAt = DateTime.UtcNow;
                _context.Semesters.Add(semester);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Semester created successfully!";
                return RedirectToAction(nameof(Semesters));
            }
            
            // Clear ModelState for IsActive to prevent "on" value issue
            ModelState.Remove("IsActive");
            return View(semester);
        }

        [HttpGet]
        public async Task<IActionResult> EditSemester(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSemester(int id, Semester semester)
        {
            if (id != semester.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(semester);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Semester updated successfully!";
                    return RedirectToAction(nameof(Semesters));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Semesters.AnyAsync(s => s.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            
            ModelState.Remove("IsActive");
            return View(semester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);
            if (semester != null)
            {
                _context.Semesters.Remove(semester);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Semester deleted successfully!";
            }
            return RedirectToAction(nameof(Semesters));
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
                TempData["SuccessMessage"] = "Course created successfully!";
                return RedirectToAction(nameof(Courses));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Semesters = new SelectList(await _context.Semesters.ToListAsync(), "Id", "Name");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Semester)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (course == null)
            {
                return NotFound();
            }
            
            ViewBag.Semesters = new SelectList(await _context.Semesters.ToListAsync(), "Id", "Name", course.SemesterId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(int id, string code, string name, string description, int credits, int semesterId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            try
            {
                var semester = await _context.Semesters.FindAsync(semesterId);
                if (semester == null)
                {
                    ModelState.AddModelError("", "Invalid semester selected.");
                    ViewBag.Semesters = new SelectList(await _context.Semesters.ToListAsync(), "Id", "Name", semesterId);
                    return View(course);
                }

                course.Code = code;
                course.Name = name;
                course.Description = description;
                course.Credits = credits;
                course.SemesterId = semesterId;

                _context.Update(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course updated successfully!";
                return RedirectToAction(nameof(Courses));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Semesters = new SelectList(await _context.Semesters.ToListAsync(), "Id", "Name", semesterId);
                return View(course);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course deleted successfully!";
            }
            return RedirectToAction(nameof(Courses));
        }

        // Class Management
        public async Task<IActionResult> Classes()
        {
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(classes);
        }

        [HttpGet]
        public async Task<IActionResult> CreateClass()
        {
            ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Semester).ToListAsync(), "Id", "Name");
            ViewBag.Teachers = new SelectList(
                await _context.Users.Where(u => u.Email != null && _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                    .ToListAsync(), 
                "Id", 
                "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClass(string name, int courseId, string teacherId, string? location, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                var teacher = await _context.Users.FindAsync(teacherId);

                if (course == null || teacher == null)
                {
                    ModelState.AddModelError("", "Invalid course or teacher selected.");
                    ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Semester).ToListAsync(), "Id", "Name");
                    ViewBag.Teachers = new SelectList(
                        await _context.Users.Where(u => u.Email != null && _context.UserRoles
                            .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                            .ToListAsync(), 
                        "Id", 
                        "Email");
                    return View();
                }

                var classEntity = new Class
                {
                    Name = name,
                    CourseId = courseId,
                    TeacherId = teacherId,
                    Location = location,
                    DayOfWeek = dayOfWeek,
                    StartTime = startTime,
                    EndTime = endTime,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Classes.Add(classEntity);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Class created successfully!";
                return RedirectToAction(nameof(Classes));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Semester).ToListAsync(), "Id", "Name");
                ViewBag.Teachers = new SelectList(
                    await _context.Users.Where(u => u.Email != null && _context.UserRoles
                        .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                        .ToListAsync(), 
                    "Id", 
                    "Email");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditClass(int id)
        {
            var classEntity = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classEntity == null)
            {
                return NotFound();
            }

            ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Semester).ToListAsync(), "Id", "Name", classEntity.CourseId);
            ViewBag.Teachers = new SelectList(
                await _context.Users.Where(u => u.Email != null && _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                    .ToListAsync(), 
                "Id", 
                "Email", 
                classEntity.TeacherId);
            return View(classEntity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClass(int id, string name, int courseId, string teacherId, string? location, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity == null)
            {
                return NotFound();
            }

            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                var teacher = await _context.Users.FindAsync(teacherId);

                if (course == null || teacher == null)
                {
                    ModelState.AddModelError("", "Invalid course or teacher selected.");
                    ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Semester).ToListAsync(), "Id", "Name", courseId);
                    ViewBag.Teachers = new SelectList(
                        await _context.Users.Where(u => u.Email != null && _context.UserRoles
                            .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                            .ToListAsync(), 
                        "Id", 
                        "Email", 
                        teacherId);
                    return View(classEntity);
                }

                classEntity.Name = name;
                classEntity.CourseId = courseId;
                classEntity.TeacherId = teacherId;
                classEntity.Location = location;
                classEntity.DayOfWeek = dayOfWeek;
                classEntity.StartTime = startTime;
                classEntity.EndTime = endTime;

                _context.Update(classEntity);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Class updated successfully!";
                return RedirectToAction(nameof(Classes));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Semester).ToListAsync(), "Id", "Name", courseId);
                ViewBag.Teachers = new SelectList(
                    await _context.Users.Where(u => u.Email != null && _context.UserRoles
                        .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                        .ToListAsync(), 
                    "Id", 
                    "Email", 
                    teacherId);
                return View(classEntity);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity != null)
            {
                _context.Classes.Remove(classEntity);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Class deleted successfully!";
            }
            return RedirectToAction(nameof(Classes));
        }

        // Teacher Management
        public async Task<IActionResult> Teachers()
        {
            var teachers = await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                .ToListAsync();
            
            // Get class counts for each teacher
            var teacherClassCounts = new Dictionary<string, int>();
            foreach (var teacher in teachers)
            {
                var classCount = await _context.Classes.CountAsync(c => c.TeacherId == teacher.Id);
                teacherClassCounts[teacher.Id] = classCount;
            }
            
            ViewBag.TeacherClassCounts = teacherClassCounts;
            return View(teachers);
        }

        [HttpGet]
        public IActionResult CreateTeacher()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeacher(string email, string firstName, string lastName, string password)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == email))
                {
                    ModelState.AddModelError("", "A user with this email already exists.");
                    return View();
                }

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Teacher");
                    TempData["SuccessMessage"] = "Teacher created successfully!";
                    return RedirectToAction(nameof(Teachers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            var teacher = await _context.Users.FindAsync(id);
            if (teacher != null)
            {
                // Check if teacher has any classes
                var hasClasses = await _context.Classes.AnyAsync(c => c.TeacherId == id);
                if (hasClasses)
                {
                    TempData["ErrorMessage"] = "Cannot delete teacher who has assigned classes.";
                }
                else
                {
                    _context.Users.Remove(teacher);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Teacher deleted successfully!";
                }
            }
            return RedirectToAction(nameof(Teachers));
        }

        // Student Management
        public async Task<IActionResult> Students()
        {
            // Get students with their enrollment counts
            var studentRoleId = (await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student"))?.Id;
            
            var students = await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == studentRoleId))
                .Select(u => new
                {
                    User = u,
                    EnrollmentCount = _context.ClassEnrollments.Count(e => e.StudentId == u.Id)
                })
                .ToListAsync();

            ViewBag.EnrollmentCounts = students.ToDictionary(s => s.User.Id, s => s.EnrollmentCount);
            
            return View(students.Select(s => s.User).ToList());
        }

        [HttpGet]
        public IActionResult CreateStudent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(string email, string firstName, string lastName, string password)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == email))
                {
                    ModelState.AddModelError("", "A user with this email already exists.");
                    return View();
                }

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Student");
                    TempData["SuccessMessage"] = "Student created successfully!";
                    return RedirectToAction(nameof(Students));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var student = await _context.Users.FindAsync(id);
            if (student != null)
            {
                _context.Users.Remove(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student deleted successfully!";
            }
            return RedirectToAction(nameof(Students));
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

        public IActionResult DownloadTemplate()
        {
            using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Students");
                    
                    // Add headers
                    worksheet.Cells[1, 1].Value = "Email";
                    worksheet.Cells[1, 2].Value = "FirstName";
                    worksheet.Cells[1, 3].Value = "LastName";
                    worksheet.Cells[1, 4].Value = "Password";
                    
                    // Style headers
                    using (var range = worksheet.Cells[1, 1, 1, 4])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    
                    // Add example row
                    worksheet.Cells[2, 1].Value = "student@example.com";
                    worksheet.Cells[2, 2].Value = "John";
                    worksheet.Cells[2, 3].Value = "Doe";
                    worksheet.Cells[2, 4].Value = "Password123!";
                    
                worksheet.Cells.AutoFitColumns();
                
                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentImportTemplate.xlsx");
            }
        }        // Enrollment Management
        public async Task<IActionResult> Enrollments()
        {
            var enrollments = await _context.ClassEnrollments
                .Include(e => e.Class)
                    .ThenInclude(c => c.Course)
                .Include(e => e.Student)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();
            return View(enrollments);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEnrollment()
        {
            ViewBag.Classes = new SelectList(
                await _context.Classes
                    .Include(c => c.Course)
                    .Select(c => new { c.Id, DisplayName = c.Course.Code + " - " + c.Name })
                    .ToListAsync(), 
                "Id", 
                "DisplayName");
            
            ViewBag.Students = new SelectList(
                await _context.Users
                    .Where(u => _context.UserRoles
                        .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Student")))
                    .Select(u => new { u.Id, DisplayName = u.Email + " (" + u.FirstName + " " + u.LastName + ")" })
                    .ToListAsync(),
                "Id",
                "DisplayName");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEnrollment(int classId, string studentId)
        {
            try
            {
                // Check if enrollment already exists
                if (await _context.ClassEnrollments.AnyAsync(e => e.ClassId == classId && e.StudentId == studentId))
                {
                    TempData["ErrorMessage"] = "Student is already enrolled in this class.";
                    return RedirectToAction(nameof(Enrollments));
                }

                var enrollment = new ClassEnrollment
                {
                    ClassId = classId,
                    StudentId = studentId,
                    EnrolledAt = DateTime.UtcNow
                };

                _context.ClassEnrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student enrolled successfully!";
                return RedirectToAction(nameof(Enrollments));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error enrolling student: {ex.Message}";
                return RedirectToAction(nameof(Enrollments));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _context.ClassEnrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.ClassEnrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Enrollment removed successfully!";
            }
            return RedirectToAction(nameof(Enrollments));
        }

        // Reports
        public async Task<IActionResult> Reports()
        {
            var stats = new
            {
                TotalSemesters = await _context.Semesters.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync(),
                TotalClasses = await _context.Classes.CountAsync(),
                TotalStudents = await _context.Users
                    .Where(u => _context.UserRoles
                        .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Student")))
                    .CountAsync(),
                TotalTeachers = await _context.Users
                    .Where(u => _context.UserRoles
                        .Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                    .CountAsync(),
                TotalEnrollments = await _context.ClassEnrollments.CountAsync(),
                TotalAttendanceSessions = await _context.AttendanceSessions.CountAsync(),
                TotalAttendanceRecords = await _context.Attendances.CountAsync(),
                ActiveSemester = await _context.Semesters
                    .Where(s => s.IsActive && s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow)
                    .FirstOrDefaultAsync()
            };

            ViewBag.Stats = stats;
            return View();
        }
    }
}
