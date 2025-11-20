using AttendanceSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }
        public DbSet<AttendanceSession> AttendanceSessions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships and constraints
            
            // Semester -> Course (One-to-Many)
            builder.Entity<Course>()
                .HasOne(c => c.Semester)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course -> Class (One-to-Many)
            builder.Entity<Class>()
                .HasOne(c => c.Course)
                .WithMany(co => co.Classes)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Teacher -> Class (One-to-Many)
            builder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Class -> ClassEnrollment (One-to-Many)
            builder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.Class)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(ce => ce.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student -> ClassEnrollment (One-to-Many)
            builder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.Student)
                .WithMany()
                .HasForeignKey(ce => ce.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Class -> AttendanceSession (One-to-Many)
            builder.Entity<AttendanceSession>()
                .HasOne(ats => ats.Class)
                .WithMany(c => c.AttendanceSessions)
                .HasForeignKey(ats => ats.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // AttendanceSession -> Attendance (One-to-Many)
            builder.Entity<Attendance>()
                .HasOne(a => a.AttendanceSession)
                .WithMany(ats => ats.Attendances)
                .HasForeignKey(a => a.AttendanceSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student -> Attendance (One-to-Many)
            builder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(u => u.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraints
            builder.Entity<Course>()
                .HasIndex(c => new { c.Code, c.SemesterId })
                .IsUnique();

            builder.Entity<ClassEnrollment>()
                .HasIndex(ce => new { ce.ClassId, ce.StudentId })
                .IsUnique();

            builder.Entity<AttendanceSession>()
                .HasIndex(ats => ats.QRCode)
                .IsUnique();
        }
    }
}
