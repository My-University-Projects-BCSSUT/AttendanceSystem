using AttendanceSystem.Data;
using AttendanceSystem.Models;
using AttendanceSystem.Patterns.Singleton;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Services
{
    public interface IStudentService
    {
        Task<List<ApplicationUser>> GetAllStudentsAsync();
        Task<List<ApplicationUser>> ImportStudentsFromExcelAsync(Stream fileStream);
        Task<ApplicationUser> GetStudentByIdAsync(string studentId);
        Task<bool> EnrollStudentInClassAsync(string studentId, int classId);
    }

    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelService _excelService;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
            _excelService = ExcelService.Instance;
        }

        public async Task<List<ApplicationUser>> GetAllStudentsAsync()
        {
            var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
            if (studentRole == null) return new List<ApplicationUser>();

            var studentIds = await _context.UserRoles
                .Where(ur => ur.RoleId == studentRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            return await _context.Users
                .Where(u => studentIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<List<ApplicationUser>> ImportStudentsFromExcelAsync(Stream fileStream)
        {
            var data = await _excelService.ReadExcelAsync(fileStream);
            var importedStudents = new List<ApplicationUser>();

            // Expected columns: Email, FirstName, LastName
            foreach (var row in data)
            {
                if (!row.ContainsKey("Email") || string.IsNullOrWhiteSpace(row["Email"]))
                    continue;

                var email = row["Email"].Trim();
                var firstName = row.ContainsKey("FirstName") ? row["FirstName"].Trim() : "Unknown";
                var lastName = row.ContainsKey("LastName") ? row["LastName"].Trim() : "Student";

                // Check if student already exists
                if (await _context.Users.AnyAsync(u => u.Email == email))
                    continue;

                var student = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                importedStudents.Add(student);
            }

            if (importedStudents.Any())
            {
                _context.Users.AddRange(importedStudents);
                await _context.SaveChangesAsync();

                // Assign Student role
                var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
                if (studentRole != null)
                {
                    var userRoles = importedStudents.Select(s => new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                    {
                        UserId = s.Id,
                        RoleId = studentRole.Id
                    });

                    _context.UserRoles.AddRange(userRoles);
                    await _context.SaveChangesAsync();
                }
            }

            return importedStudents;
        }

        public async Task<ApplicationUser> GetStudentByIdAsync(string studentId)
        {
            var student = await _context.Users.FindAsync(studentId);
            if (student == null)
                throw new InvalidOperationException("Student not found.");
            
            return student;
        }

        public async Task<bool> EnrollStudentInClassAsync(string studentId, int classId)
        {
            // Check if already enrolled
            if (await _context.ClassEnrollments.AnyAsync(e => e.StudentId == studentId && e.ClassId == classId))
                return false;

            var enrollment = new ClassEnrollment
            {
                StudentId = studentId,
                ClassId = classId,
                EnrolledAt = DateTime.UtcNow
            };

            _context.ClassEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
