# Attendance System - Project Summary

## ✅ Completed Implementation (Final Version)

### Core Features Implemented

1. **User Management**
   - Three user roles: Admin, Teacher, Student
   - ASP.NET Core Identity for authentication
   - Google OAuth integration for students only (with clear labeling)
   - Email/password login for Admin and Teachers

2. **Admin Dashboard** (Full CRUD)
   - Create and manage semesters
   - Create and manage courses (using Builder Pattern)
   - Create and manage classes
   - Create and manage teachers
   - Create and manage students
   - Import students from Excel files (EPPlus 7.5.0)
   - Download Excel template
   - Manage student enrollments
   - View system-wide reports and statistics

3. **Teacher Dashboard**
   - View assigned classes with enrollment counts
   - Start attendance sessions
   - Generate QR codes with JSON format
   - Display session codes for manual entry (with copy button)
   - Time-limited QR codes (15-minute expiration with countdown)
   - Manually close sessions
   - Auto-expire sessions after 15 minutes
   - Mark absent students automatically
   - View attendance records grouped by session
   - Session statistics (Present, Late, Absent, Excused counts)
   - Teacher-specific reports

4. **Student Portal**
   - Login with email/password OR Google account (Students Only)
   - **Dual QR scanning modes**:
     - Camera scan with html5-qrcode library (auto-submit)
     - Manual session code entry
   - JSON QR code parsing (extracts code from JSON)
   - View attendance history grouped by session
   - View enrolled classes with schedules
   - Attendance statistics dashboard

## 🎨 Design Patterns Implemented

### 1. Factory Pattern
**Location:** `Patterns/Factory/UserFactory.cs`

**Purpose:** Creates users with proper role assignment
```csharp
// Usage example:
var user = await _userFactory.CreateUserAsync(
    email: "teacher@example.com",
    firstName: "John",
    lastName: "Doe",
    role: "Teacher",
    password: "Teacher@123"
);
```

### 2. Builder Pattern
**Location:** 
- `Patterns/Builder/AttendanceSessionBuilder.cs`
- `Patterns/Builder/CourseBuilder.cs`

**Purpose:** Constructs complex objects step-by-step
```csharp
// Attendance Session Builder example:
var session = _sessionBuilder
    .SetClass(classEntity)
    .SetSessionDate(DateTime.UtcNow)
    .SetQRCodeExpiration(15) // 15 minutes
    .SetActive(true)
    .Build();

// Course Builder example:
var course = _courseBuilder
    .SetCode("COS20007")
    .SetName("Object Oriented Programming")
    .SetDescription("Learn OOP concepts")
    .SetCredits(12)
    .SetSemester(semester)
    .Build();
```

### 3. Singleton Pattern
**Location:** 
- `Patterns/Singleton/QRCodeService.cs`
- `Patterns/Singleton/ExcelService.cs`

**Purpose:** Ensures single instance of services
```csharp
// QR Code Service (generates JSON QR data)
var qrService = QRCodeService.Instance;
var qrData = qrService.GenerateAttendanceQRData(sessionId, code);
var qrCodeBase64 = qrService.GenerateQRCodeBase64(qrData);

// Excel Service (EPPlus 7.5.0 with NonCommercial license)
var excelService = ExcelService.Instance;
var students = await excelService.ReadExcelAsync(filePath);
```

## 📊 Database Schema

```
ApplicationUser (Identity)
├── Id (string)
├── FirstName (string)
├── LastName (string)
├── Email (string)
└── [Attendances] (navigation)

Semester
├── Id (int)
├── Name (string)
├── StartDate (DateTime)
├── EndDate (DateTime)
└── [Courses] (navigation)

Course
├── Id (int)
├── Code (string)
├── Name (string)
├── Credits (int)
├── SemesterId (int)
└── [Classes] (navigation)

Class
├── Id (int)
├── Name (string)
├── CourseId (int)
├── TeacherId (string)
├── DayOfWeek (enum)
├── StartTime (TimeSpan)
├── EndTime (TimeSpan)
├── [Enrollments] (navigation)
└── [AttendanceSessions] (navigation)

ClassEnrollment
├── Id (int)
├── ClassId (int)
├── StudentId (string)
└── EnrolledAt (DateTime)

AttendanceSession
├── Id (int)
├── ClassId (int)
├── SessionDate (DateTime)
├── QRCode (string)
├── QRCodeExpiresAt (DateTime)
├── IsActive (bool)
└── [Attendances] (navigation)

Attendance
├── Id (int)
├── AttendanceSessionId (int)
├── StudentId (string)
├── Status (enum: Present, Late, Absent, Excused)
├── CheckInTime (DateTime?)
└── Notes (string?)
```

## 🚀 How to Run the Application

### 1. Start SQL Server (Docker)
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin@123" \
  -p 1433:1433 --name sqlserver -d mcr.microsoft.com/azure-sql-edge
```

### 2. Build and Run
```bash
cd AttendanceSystem

# Restore packages
dotnet restore

# Apply migrations (creates database)
dotnet ef database update

# Run the application
dotnet run
```

### 3. Access the Application
- **URL:** https://localhost:7223 or http://localhost:5130
- **Default Admin:**
  - Email: admin@attendance.com
  - Password: Admin@123

### 4. Configure Google Authentication (Optional for Students)
1. Get credentials from Google Cloud Console
2. Update `appsettings.json`:
```json
"Authentication": {
  "Google": {
    "ClientId": "YOUR_CLIENT_ID.apps.googleusercontent.com",
    "ClientSecret": "YOUR_CLIENT_SECRET"
  }
}
```
3. Add redirect URIs in Google Console:
   - `http://localhost:5130/signin-google`
   - `https://localhost:7223/signin-google`

## 📝 Typical Workflows

### Admin Workflow
1. Login as admin
2. Create a new semester (e.g., "Semester 1 2025")
3. Create courses for that semester
4. Create classes for courses and assign teachers
5. Import students from Excel or create manually
6. Enroll students in classes

### Teacher Workflow
1. Login with teacher credentials
2. Navigate to "My Classes"
3. Click on a class to view details
4. Click "Start Attendance Session"
5. Display QR code to students
6. Monitor real-time attendance

### Student Workflow
1. Login with email/password OR Google account (auto-registers as student)
2. Navigate to "Scan QR Code"
3. **Option 1 (Camera)**: Click "Start Camera Scanner" and point at QR code
4. **Option 2 (Manual)**: Enter session code displayed by teacher
5. System automatically:
   - Parses JSON QR code data
   - Validates session (not expired, student enrolled)
   - Records attendance (Present or Late based on time)
   - Redirects to success page
6. View attendance history with statistics

## 📦 Installed Packages

```xml
<!-- Core Framework -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.0" />

<!-- Authentication -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="9.0.0" />

<!-- QR Code Generation -->
<PackageReference Include="QRCoder" Version="1.7.0" />
<PackageReference Include="System.Drawing.Common" Version="10.0.0" />

<!-- Excel Processing -->
<PackageReference Include="EPPlus" Version="7.5.0" />
<!-- Note: Downgraded from 8.2.1 to 7.5.0 for free non-commercial use -->
<!-- License: ExcelPackage.LicenseContext = LicenseContext.NonCommercial -->
```

### Client-Side Libraries
- **html5-qrcode 2.3.8**: QR code scanning in browser
- **Bootstrap 5**: Responsive UI framework
- **Font Awesome 6.4.0**: Icon library

## 📋 Excel Import Format for Students

Create an Excel file with these columns:

| Email | FirstName | LastName | Password |
|-------|-----------|----------|----------|
| student1@newinti.edu.my | John | Doe | Student@123 |
| student2@newinti.edu.my | Jane | Smith | Student@456 |

**Download Template**: Admin → Students → Import Students → "Download Template" button

## 🔐 Security Features

- ✅ Role-based authorization (Admin, Teacher, Student)
- ✅ Password requirements (8+ chars, uppercase, lowercase, digit, special)
- ✅ Google OAuth (Students Only - clearly labeled)
- ✅ Admin and Teachers must use email/password
- ✅ Time-limited QR codes (15-minute expiration with countdown timer)
- ✅ Automatic session expiration
- ✅ Automatic absent marking for non-attending students
- ✅ Anti-forgery tokens on forms
- ✅ Enrollment validation before attendance submission
- ✅ Secure password hashing with Identity
- ✅ JSON QR code format for structured data
- ✅ Timezone handling (UTC storage, local time display)

## 🏗️ OOP Principles Demonstrated

### Encapsulation
- Private fields with public properties
- Data validation in setters
- Protected navigation properties

### Inheritance
- `ApplicationUser` extends `IdentityUser`
- All models inherit common patterns

### Polymorphism
- Interface-based services (`IAttendanceService`, `IStudentService`)
- Factory and Builder interfaces

### Abstraction
- Service interfaces hide implementation details
- Repository pattern through EF Core DbContext

## 📁 Project Structure

```
AttendanceSystem/
├── Controllers/              # MVC Controllers
│   ├── AccountController.cs  # Login, Logout, Google Auth
│   ├── AdminController.cs    # Admin CRUD operations
│   ├── TeacherController.cs  # Teacher QR codes and reports
│   └── StudentController.cs  # Student attendance submission
├── Models/                   # Domain Models
│   ├── User.cs              # ApplicationUser (Identity)
│   ├── Semester.cs
│   ├── Course.cs
│   ├── Class.cs
│   ├── ClassEnrollment.cs
│   ├── AttendanceSession.cs
│   └── Attendance.cs
├── Patterns/                 # Design Patterns
│   ├── Factory/
│   │   └── UserFactory.cs   # Factory Pattern
│   ├── Builder/
│   │   ├── AttendanceSessionBuilder.cs  # Builder Pattern
│   │   └── CourseBuilder.cs             # Builder Pattern
│   └── Singleton/
│       ├── QRCodeService.cs     # Singleton Pattern
│       └── ExcelService.cs      # Singleton Pattern
├── Services/                 # Business Logic Layer
│   ├── AttendanceService.cs
│   └── StudentService.cs
├── Data/                     # Data Access Layer
│   ├── ApplicationDbContext.cs  # EF Core DbContext
│   └── DbInitializer.cs         # Database seeding
├── Views/                    # Razor Views (auto-generated)
├── Program.cs               # Application entry point
├── appsettings.json         # Configuration
└── README.md                # Documentation
```

## ⚙️ Recent Updates & Bug Fixes

### QR Code Scanning
- ✅ Fixed JSON QR code parsing (extracts code from `{"type":"attendance","sessionId":20,"code":"uuid"}`)
- ✅ Implemented dual-mode scanning (camera + manual entry)
- ✅ Auto-submit after successful QR detection
- ✅ Camera permission handling and error messages

### Session Management
- ✅ Added automatic session expiration after 15 minutes
- ✅ Implemented auto-marking of absent students
- ✅ Added manual "Close Session Now" button for teachers
- ✅ Session status indicators (Active/Inactive)
- ✅ Session code display with copy-to-clipboard

### UI/UX Improvements
- ✅ Grouped attendance records by session
- ✅ Per-session statistics (Present, Late, Absent, Excused counts)
- ✅ Countdown timer with auto-refresh on QR page
- ✅ Success/error messages with TempData
- ✅ Clear labeling: "Google login (Students Only)"
- ✅ Excel template download button

### Timezone & Data
- ✅ UTC storage with `.ToLocalTime()` display (Malaysia UTC+8)
- ✅ JavaScript countdown uses 'Z' suffix for UTC parsing
- ✅ Fixed enrollment count display (dynamic query instead of hardcoded "0")
- ✅ EPPlus downgrade to 7.5.0 for license compatibility

### Database
- ✅ Migrated from SQLite to SQL Server (Azure SQL Edge in Docker)
- ✅ Code First approach with EF Core migrations
- ✅ Connection string: `Server=localhost,1433;Database=AttendanceSystemDb`

## ⚙️ Next Steps & Future Enhancements

### Immediate Tasks
- ✅ All views created and functional
- ✅ QR scanning with camera and manual entry working
- ✅ Session expiration and absent marking implemented
- ✅ Google OAuth configured (optional setup)

### Future Enhancements
1. **Reports:** Export attendance to Excel/PDF
2. **Email Notifications:** Send absence notifications to students
3. **Dashboard:** Real-time attendance statistics with charts
4. **Mobile App:** Native iOS/Android app for students
5. **Biometric:** Add fingerprint/face recognition option
6. **Analytics:** Attendance trends and insights over time
7. **Multi-language:** Support for multiple languages
8. **SMS Integration:** Text message notifications

## 🐛 Troubleshooting

### QR Scanning Issues
**Problem:** Camera closes but attendance not submitted
**Solution:** QR codes are JSON format. Updated JavaScript to parse JSON and extract code field.

### Session Expiration
**Problem:** Sessions stay "Active" forever
**Solution:** Added auto-expiration in ViewAttendance action and manual close button.

### EPPlus License Error
**Problem:** EPPlus 8.x requires commercial license
**Solution:** Downgraded to EPPlus 7.5.0 with `LicenseContext.NonCommercial`.

### Timezone Display
**Problem:** Times showing in UTC instead of local time
**Solution:** Use `.ToLocalTime()` for display, JavaScript uses 'Z' suffix for UTC parsing.

### Google Login Redirect Loop
**Problem:** After Google login, redirects back to login page
**Solution:** Added error messages, link existing accounts, auto-create with Student role.

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Database Issues
```bash
# Recreate database
dotnet ef database drop
dotnet ef database update
```

### Docker SQL Server
```bash
# Stop and remove container
docker stop sqlserver
docker rm sqlserver

# Start fresh
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin@123" \
  -p 1433:1433 --name sqlserver -d mcr.microsoft.com/azure-sql-edge
```

## 📞 Support & Documentation

- **Full README:** See `README.md` in the project root
- **API Documentation:** Generate with Swagger (add Swashbuckle package)
- **Code Comments:** All classes and methods are documented

## ✨ Key Highlights

1. ✅ **Full OOP Implementation** - Proper use of classes, inheritance, encapsulation, polymorphism
2. ✅ **Three Creational Patterns** - Factory, Builder, Singleton (properly implemented)
3. ✅ **Role-Based Security** - Admin, Teacher, Student roles with proper authorization
4. ✅ **Modern Architecture** - MVC pattern with service layer separation
5. ✅ **Google Integration** - OAuth 2.0 for students (clearly labeled, optional)
6. ✅ **Advanced QR System** - JSON format, dual scanning modes, auto-expiration
7. ✅ **Session Management** - Auto-expiration, absent marking, manual close
8. ✅ **Excel Import/Export** - Bulk student upload with template download
9. ✅ **SQL Server with Docker** - Production-ready database setup
10. ✅ **Timezone Handling** - UTC storage, local time display (Malaysia UTC+8)
11. ✅ **Comprehensive UI** - All CRUD operations with Bootstrap 5
12. ✅ **Error Handling** - TempData messages, validation, try-catch blocks
13. ✅ **Git Repository** - Version controlled with proper .gitignore
14. ✅ **Complete Documentation** - README, PROJECT_SUMMARY, inline comments

---

**Project Status:** ✅ **COMPLETE** and fully functional!

**Final Version Features:**
- ✅ All views created (Admin: 13, Teacher: 5, Student: 4, Account: 3)
- ✅ QR scanning with camera + manual entry working
- ✅ Session auto-expiration and absent marking implemented
- ✅ Timezone conversion throughout (UTC → Malaysia time)
- ✅ EPPlus 7.5.0 with proper licensing
- ✅ JSON QR code parsing
- ✅ Google OAuth (Students Only) clearly labeled
- ✅ Full CRUD operations for all entities
- ✅ Reports and statistics dashboards
- ✅ Excel import/export functionality

The project successfully demonstrates OOP principles and Creational Design Patterns in a real-world attendance management system with production-ready features.

**Technologies:** .NET 9.0 | ASP.NET Core MVC | EF Core 9.0 | SQL Server | ASP.NET Identity | Google OAuth | QRCoder | EPPlus 7.5.0 | html5-qrcode | Bootstrap 5

**University:** Swinburne University of Technology
**Course:** COS20007 - Object Oriented Programming  
**Year:** 2025 | Semester 1
