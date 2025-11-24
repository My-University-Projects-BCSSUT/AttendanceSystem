# 🚀 Quick Start Guide - Attendance System

## Default Login Credentials

```
Admin Account:
Email: admin@attendance.com
Password: Admin@123
```

## Running the Application

```bash
cd AttendanceSystem/AttendanceSystem
dotnet run
```

Then open your browser to: `https://localhost:5001`

## Design Patterns Quick Reference

### 1️⃣ Factory Pattern - `UserFactory.cs`
**What it does:** Creates users with specific roles (Admin, Teacher, Student)

**Example Usage in AdminController:**
```csharp
var user = await _userFactory.CreateUserAsync(
    email: "teacher@school.com",
    firstName: "John",
    lastName: "Doe",
    role: "Teacher"
);
```

### 2️⃣ Builder Pattern - `CourseBuilder.cs` & `AttendanceSessionBuilder.cs`

**CourseBuilder - What it does:** Constructs courses step-by-step with validation

**Example Usage in AdminController:**
```csharp
var course = _courseBuilder
    .SetCode("COS20007")
    .SetName("Object Oriented Programming")
    .SetDescription("Learn OOP concepts")
    .SetCredits(12)
    .SetSemester(semester)
    .Build();
```

**AttendanceSessionBuilder - What it does:** Creates attendance sessions with QR codes

**Example Usage in AttendanceService:**
```csharp
var session = _sessionBuilder
    .SetClass(classEntity)
    .SetSessionDate(DateTime.UtcNow)
    .SetQRCodeExpiration(15) // 15 minutes
    .SetActive(true)
    .Build();
```

### 3️⃣ Singleton Pattern - `QRCodeService.cs` & `ExcelService.cs`

**QRCodeService - What it does:** Generates QR codes (single instance shared)

**Example Usage in TeacherController:**
```csharp
var qrService = QRCodeService.Instance;
var qrCodeBase64 = qrService.GenerateQRCodeBase64(data);
```

**ExcelService - What it does:** Imports/exports Excel files (single instance shared)

**Example Usage in StudentService:**
```csharp
var excelService = ExcelService.Instance;
var data = await excelService.ReadExcelAsync(fileStream);
```

## OOP Principles Demonstrated

### ✅ Encapsulation
```csharp
// Private fields, public properties
public class Semester
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    private List<Course> _courses; // Private backing field
    
    // Encapsulated method
    public bool IsCurrentSemester()
    {
        var now = DateTime.UtcNow;
        return now >= StartDate && now <= EndDate && IsActive;
    }
}
```

### ✅ Inheritance
```csharp
// ApplicationUser inherits from IdentityUser
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    // Inherited properties from IdentityUser:
    // - Id, UserName, Email, PasswordHash, etc.
}
```

### ✅ Polymorphism
```csharp
// Interface-based polymorphism
public interface IAttendanceService
{
    Task<AttendanceSession> CreateSessionAsync(int classId, int expirationMinutes = 15);
    Task<Attendance> SubmitAttendanceAsync(int sessionId, string studentId, string qrCode);
}

// Implementation can be swapped
public class AttendanceService : IAttendanceService
{
    // Implementation details...
}
```

### ✅ Abstraction
```csharp
// Abstract service interfaces hide implementation
IUserFactory -> UserFactory
IAttendanceService -> AttendanceService
IStudentService -> StudentService
```

## Key Features by Role

### 👨‍💼 Admin Can:
- ✅ Create/manage semesters
- ✅ Create/manage courses (using Builder Pattern)
- ✅ Create/manage classes
- ✅ Import students from Excel (using Singleton)
- ✅ Create teachers/students (using Factory Pattern)
- ✅ Enroll students in classes

### 👨‍🏫 Teacher Can:
- ✅ View assigned classes
- ✅ Start attendance sessions
- ✅ Generate QR codes (using Singleton)
- ✅ View student attendance records
- ✅ Monitor real-time attendance

### 👨‍🎓 Student Can:
- ✅ Login with Google (auto-registers)
- ✅ Scan QR codes
- ✅ Submit attendance
- ✅ View attendance confirmation

## Testing the Workflow

### Step 1: Admin Setup
1. Login as admin (admin@attendance.com / Admin@123)
2. Create a semester: "Semester 1 2025" (Jan 1 - Jun 30, 2025)
3. Create a course: COS20007 - Object Oriented Programming
4. Create a class: "Tutorial 1" (Monday 10:00-12:00)
5. Import students or create manually

### Step 2: Teacher Uses System
1. Login as teacher (create teacher account first)
2. Navigate to "My Classes"
3. Select "Tutorial 1"
4. Click "Start Attendance Session"
5. Show QR code on screen

### Step 3: Student Marks Attendance
1. Login with Google account
2. Scan the QR code
3. System validates and records attendance
4. Shows confirmation

## Project Structure Overview

```
AttendanceSystem/
│
├── Models/              # Domain entities (User, Course, Class, etc.)
│   └── [OOP: Encapsulation, Inheritance]
│
├── Patterns/           # Design patterns implementation
│   ├── Factory/       # Factory Pattern
│   ├── Builder/       # Builder Pattern
│   └── Singleton/     # Singleton Pattern
│
├── Services/          # Business logic layer
│   └── [OOP: Abstraction, Polymorphism]
│
├── Data/              # Data access layer (EF Core)
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
│
└── Controllers/       # MVC Controllers
    ├── AccountController.cs
    ├── AdminController.cs
    ├── TeacherController.cs
    └── StudentController.cs
```

## Common Commands

```bash
# Build the project
dotnet build

# Run the project
dotnet run

# Restore packages
dotnet restore

# Clean build artifacts
dotnet clean

# Create migration (if needed)
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

## Configuration Files

### appsettings.json
- Database connection string (SQLite)
- Google OAuth credentials
- Logging configuration

### Program.cs
- Service registration (Dependency Injection)
- Identity configuration
- Middleware pipeline
- Database initialization

## Important URLs

- **Home:** https://localhost:5001
- **Login:** https://localhost:5001/Account/Login
- **Admin:** https://localhost:5001/Admin
- **Teacher:** https://localhost:5001/Teacher
- **Student:** https://localhost:5001/Student

## Excel Import Format

Create `students.xlsx` with:

| Email | FirstName | LastName |
|-------|-----------|----------|
| student1@uni.edu | John | Doe |
| student2@uni.edu | Jane | Smith |

Upload via: Admin → Import Students

## Need Help?

- **README:** See `AttendanceSystem/README.md`
- **Summary:** See `PROJECT_SUMMARY.md`
- **Code Comments:** All files are documented

---

**✅ Project Complete!** All OOP principles and Creational Design Patterns implemented successfully.
