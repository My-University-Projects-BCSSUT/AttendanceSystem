# Attendance System

A comprehensive .NET MVC Attendance Management System built with OOP principles and Creational Design Patterns.

## 🎓 Features

### User Roles
- **Admin**: Manage semesters, courses, classes, teachers, and students
- **Teacher**: Generate QR codes, track attendance, and view reports
- **Student**: Scan QR codes (camera or manual) and view attendance history

### Key Functionalities
- **Admin Dashboard**
  - Full CRUD for semesters, courses, classes, teachers, and students
  - Import students via Excel files (bulk upload)
  - Manage student enrollments
  - Generate reports and statistics
  
- **Teacher Dashboard**
  - View assigned classes and enrolled students
  - Generate time-limited QR codes (15-minute expiration)
  - Display session codes for manual entry
  - View attendance records grouped by session
  - Real-time attendance reports with statistics
  - Manually close sessions and mark absent students
  
- **Student Portal**
  - Login with Google account (Students Only)
  - Dual QR scanning modes: Camera scan or Manual entry
  - View attendance history with statistics
  - View enrolled classes and schedules

## 🏗️ Architecture & Design Patterns

### OOP Principles Implemented
- **Encapsulation**: Data hiding with proper access modifiers
- **Inheritance**: Base `ApplicationUser` class extended by role-specific users
- **Polymorphism**: Interface-based service implementations
- **Abstraction**: Abstract interfaces for services and factories

### Creational Design Patterns

#### 1. Factory Pattern (`UserFactory`)
```csharp
IUserFactory -> UserFactory
```
- Creates users based on roles (Admin, Teacher, Student)
- Handles role assignment and password generation
- Ensures consistent user creation across the application

#### 2. Builder Pattern
```csharp
IAttendanceSessionBuilder -> AttendanceSessionBuilder
ICourseBuilder -> CourseBuilder
```
- **AttendanceSessionBuilder**: Constructs complex attendance sessions with QR codes
- **CourseBuilder**: Creates courses with validation and proper relationships

#### 3. Singleton Pattern
```csharp
QRCodeService.Instance
ExcelService.Instance
```
- **QRCodeService**: Thread-safe singleton for QR code generation
- **ExcelService**: Single instance for Excel import/export operations (EPPlus 7.5.0)

## 🛠️ Technology Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core MVC** - Web application framework
- **Entity Framework Core 9.0** - ORM for database operations
- **SQL Server (Azure SQL Edge)** - Docker containerized database
- **ASP.NET Core Identity** - Authentication & Authorization
- **Google OAuth 2.0** - External authentication for students
- **QRCoder 1.7.0** - QR code generation library
- **EPPlus 7.5.0** - Excel file processing (non-commercial license)
- **html5-qrcode 2.3.8** - Client-side QR code scanning
- **Bootstrap 5** - Responsive UI framework
- **Font Awesome 6.4.0** - Icons

## 📦 Installation & Setup

### Prerequisites
- .NET 9.0 SDK or later
- Docker Desktop (for SQL Server)
- Visual Studio 2022 / VS Code / Rider

### Step 1: Clone the Repository
```bash
git clone <repository-url>
cd AttendanceSystem
```

### Step 2: Start SQL Server with Docker
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin@123" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/azure-sql-edge
```

### Step 3: Restore NuGet Packages
```bash
cd AttendanceSystem
dotnet restore
```

### Step 4: Apply Database Migrations
```bash
dotnet ef database update
```

The database will be automatically seeded with:
- Default admin account
- Sample data (if needed)

### Step 5: Configure Google Authentication (Optional for Students)

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Enable Google+ API
4. Create OAuth 2.0 credentials
5. Add authorized redirect URIs:
   - `https://localhost:7223/signin-google`
   - `http://localhost:5130/signin-google`
6. Update `appsettings.json`:

```json
"Authentication": {
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
  }
}
```

### Step 6: Run the Application
```bash
dotnet run
```

Navigate to `https://localhost:7223` or `http://localhost:5130`

## 👤 Default Login Credentials

- **Admin**: admin@attendance.com / Admin@123
- **Teachers & Students**: Created by admin via dashboard

## 📁 Project Structure

```
AttendanceSystem/
├── Controllers/           # MVC Controllers
│   ├── AccountController.cs     # Login, Logout, Google OAuth
│   ├── AdminController.cs       # Admin CRUD operations
│   ├── TeacherController.cs     # QR codes, attendance, reports
│   └── StudentController.cs     # QR scanning, attendance submission
├── Models/               # Domain Models
│   ├── ApplicationUser.cs       # User entity (Identity)
│   ├── Semester.cs
│   ├── Course.cs
│   ├── Class.cs
│   ├── ClassEnrollment.cs
│   ├── AttendanceSession.cs
│   └── Attendance.cs
├── Patterns/             # Design Patterns
│   ├── Factory/
│   │   └── UserFactory.cs       # Factory Pattern
│   ├── Builder/
│   │   ├── AttendanceSessionBuilder.cs  # Builder Pattern
│   │   └── CourseBuilder.cs             # Builder Pattern
│   └── Singleton/
│       ├── QRCodeService.cs     # Singleton Pattern
│       └── ExcelService.cs      # Singleton Pattern
├── Services/             # Business Logic
│   ├── AttendanceService.cs     # Attendance operations
│   └── StudentService.cs        # Student operations
├── Data/                 # Data Layer
│   ├── ApplicationDbContext.cs  # EF Core DbContext
│   └── DbInitializer.cs         # Database seeding
├── Views/               # Razor Views
│   ├── Admin/           # Admin portal views
│   ├── Teacher/         # Teacher portal views
│   ├── Student/         # Student portal views
│   ├── Account/         # Login/logout views
│   └── Shared/          # Layout and partials
└── wwwroot/             # Static files
```

## 📊 Database Schema

```
Users (ApplicationUser)
  └─> Attendances
  
Semesters
  └─> Courses
      └─> Classes
          ├─> ClassEnrollments
          └─> AttendanceSessions
              └─> Attendances
```

**Connection String:**
```
Server=localhost,1433;Database=AttendanceSystemDb;User Id=sa;Password=Admin@123;TrustServerCertificate=True;Encrypt=False;
```

## 🔐 Security Features

- Role-based authorization with ASP.NET Identity
- Password requirements enforcement
- External authentication via Google OAuth (Students Only)
- Time-limited QR codes (15-minute expiration with countdown)
- Automatic session expiration and absent marking
- Anti-forgery token validation
- Secure password hashing
- Timezone handling (UTC storage, Malaysia time display)

## 📱 Usage Workflow

### Admin Workflow
1. Login with admin credentials
2. Create semesters (e.g., "Semester 1 2025")
3. Create courses for that semester
4. Create classes and assign teachers
5. Create teacher accounts
6. Import students via Excel or create manually
7. Enroll students in classes
8. View system-wide reports

### Teacher Workflow
1. Login with teacher account
2. View assigned classes
3. Start an attendance session
4. Display QR code (with session code for manual entry)
5. Monitor attendance in real-time
6. Manually close session or wait for auto-expiration
7. View attendance reports grouped by session

### Student Workflow
1. Login with email/password OR Google account (auto-creates student)
2. **Option 1**: Scan QR code with camera (automatic submission)
3. **Option 2**: Manually enter session code
4. View attendance confirmation
5. Check attendance history and statistics

## 📋 Excel Import Format

Students can be imported using Excel files with these columns:

| Email | FirstName | LastName | Password |
|-------|-----------|----------|----------|
| student@newinti.edu.my | John | Doe | Pass@123 |

Download the template from Admin → Students → Import Students → Download Template

## ✨ Key Features Implemented

### QR Code Attendance
- **JSON-based QR codes**: `{"type":"attendance","sessionId":20,"code":"uuid"}`
- **Dual scanning modes**: Camera scan or manual code entry
- **Auto-parsing**: JavaScript extracts code from JSON QR data
- **15-minute expiration**: Countdown timer with auto-refresh
- **Session code display**: Teachers can share code for manual entry
- **Copy to clipboard**: One-click copy button for session codes

### Session Management
- **Auto-expiration**: Sessions automatically expire after 15 minutes
- **Manual close**: Teachers can close sessions early
- **Absent marking**: Students who don't check in are marked absent
- **Active/Inactive status**: Clear session state indicators

### Attendance Tracking
- **Real-time updates**: Attendance records visible immediately
- **Status types**: Present, Late, Absent, Excused
- **Late detection**: 15-minute grace period after class start time
- **Session grouping**: Attendance records grouped by session
- **Statistics**: Present, Late, Absent, Excused counts per session

### Timezone Handling
- **UTC storage**: All timestamps stored in UTC in database
- **Local display**: Converted to Malaysia time (UTC+8) using `.ToLocalTime()`
- **JavaScript parsing**: Countdown timers use 'Z' suffix for UTC parsing

## 🧪 Testing Scenarios

### Test QR Code Scanning
1. Admin creates teacher and student
2. Admin enrolls student in class
3. Teacher starts attendance session
4. Teacher displays QR code
5. Student scans with camera → Attendance recorded
6. Student scans again → Error: "Attendance already submitted"
7. Wait 15 minutes → Session expires, QR invalid

### Test Session Expiration
1. Teacher starts session
2. Some students scan QR
3. Wait 15 minutes OR teacher clicks "Close Session Now"
4. View attendance → Students who didn't scan marked as absent
5. Session shows as "Inactive"

### Test Excel Import
1. Admin downloads student template
2. Fill in student data (Email, FirstName, LastName, Password)
3. Upload Excel file
4. Students can login with provided credentials

## 🚀 Deployment

### Connection String for Production
Update `appsettings.json` with your production SQL Server:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=AttendanceSystemDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```

### Deploy to Azure
```bash
az webapp up --name attendance-system --resource-group myResourceGroup
```

### Deploy to IIS
1. Publish: `dotnet publish -c Release`
2. Copy files to IIS directory
3. Configure application pool for .NET Core
4. Update connection strings

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📝 License

This project is for educational purposes as part of COS20007 coursework.

## 👨‍💻 Author

Developed for Swinburne University - COS20007 Object Oriented Programming

## 🐛 Known Issues & Solutions

### Issue: QR code scanning doesn't work
**Solution**: QR codes are JSON formatted. The system now automatically parses JSON and extracts the code.

### Issue: Session still shows as "Active" after expiration
**Solution**: View attendance page now automatically expires sessions and marks absent students.

### Issue: EPPlus license error
**Solution**: Downgraded to EPPlus 7.5.0 with `LicenseContext.NonCommercial` setting.

### Issue: Timezone showing UTC instead of local time
**Solution**: All displays use `.ToLocalTime()` for Malaysia time conversion.

## 📞 Support

For issues or questions, please create an issue in the repository.
