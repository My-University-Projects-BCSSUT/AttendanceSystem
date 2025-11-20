# Attendance System

A comprehensive .NET MVC Attendance Management System built with OOP principles and Creational Design Patterns.

## 🎓 Features

### User Roles
- **Admin**: Manage semesters, courses, classes, and students
- **Teacher**: View students, generate QR codes, and track attendance
- **Student**: Scan QR codes and submit attendance via Google authentication

### Key Functionalities
- **Admin Dashboard**
  - Create and manage semesters, courses, and classes
  - Import students via Excel files
  - Enroll students in classes
  
- **Teacher Dashboard**
  - View assigned classes and enrolled students
  - Generate time-limited QR codes for attendance sessions
  - View real-time attendance reports
  
- **Student Portal**
  - Login with Google account
  - Scan QR codes to mark attendance
  - View attendance history

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
- **ExcelService**: Single instance for Excel import/export operations

## 🛠️ Technology Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core MVC** - Web application framework
- **Entity Framework Core** - ORM for database operations
- **SQLite** - Lightweight database (can be changed to SQL Server)
- **ASP.NET Core Identity** - Authentication & Authorization
- **Google OAuth 2.0** - External authentication for students
- **QRCoder** - QR code generation library
- **EPPlus** - Excel file processing

## 📦 Installation & Setup

### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio 2022 / VS Code / Rider

### Step 1: Clone the Repository
```bash
git clone <repository-url>
cd AttendanceSystem
```

### Step 2: Restore NuGet Packages
```bash
cd AttendanceSystem
dotnet restore
```

### Step 3: Configure Google Authentication

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Enable Google+ API
4. Create OAuth 2.0 credentials
5. Add authorized redirect URIs:
   - `https://localhost:5001/signin-google`
   - `http://localhost:5000/signin-google`
6. Update `appsettings.json`:

```json
"Authentication": {
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
  }
}
```

### Step 4: Run Database Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 5: Run the Application
```bash
dotnet run
```

Navigate to `https://localhost:5001` or `http://localhost:5000`

## 👤 Default Admin Credentials

- **Email**: admin@attendance.com
- **Password**: Admin@123

## 📁 Project Structure

```
AttendanceSystem/
├── Controllers/           # MVC Controllers
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── TeacherController.cs
│   └── StudentController.cs
├── Models/               # Domain Models
│   ├── User.cs
│   ├── Semester.cs
│   ├── Course.cs
│   ├── Class.cs
│   ├── ClassEnrollment.cs
│   ├── AttendanceSession.cs
│   └── Attendance.cs
├── Patterns/             # Design Patterns
│   ├── Factory/
│   │   └── UserFactory.cs
│   ├── Builder/
│   │   ├── AttendanceSessionBuilder.cs
│   │   └── CourseBuilder.cs
│   └── Singleton/
│       ├── QRCodeService.cs
│       └── ExcelService.cs
├── Services/             # Business Logic
│   ├── AttendanceService.cs
│   └── StudentService.cs
├── Data/                 # Data Layer
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
└── Views/               # Razor Views
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

## 🔐 Security Features

- Role-based authorization with ASP.NET Identity
- Password requirements enforcement
- External authentication via Google OAuth
- Time-limited QR codes (15-minute expiration)
- Anti-forgery token validation
- Secure password hashing

## 📱 Usage Workflow

### Admin Workflow
1. Login with admin credentials
2. Create semesters and courses
3. Create classes and assign teachers
4. Import students via Excel or create manually
5. Enroll students in classes

### Teacher Workflow
1. Login with teacher account
2. View assigned classes
3. Start an attendance session
4. Display QR code to students
5. Monitor attendance in real-time

### Student Workflow
1. Login with Google account (auto-creates student account)
2. Scan displayed QR code
3. Submit attendance
4. View confirmation

## 📋 Excel Import Format

Students can be imported using Excel files with the following columns:

| Email | FirstName | LastName |
|-------|-----------|----------|
| student@example.com | John | Doe |

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 🚀 Deployment

### Deploy to Azure
```bash
az webapp up --name attendance-system --resource-group myResourceGroup
```

### Deploy to IIS
1. Publish the application: `dotnet publish -c Release`
2. Copy published files to IIS directory
3. Configure IIS application pool for .NET Core
4. Update connection strings in production

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📝 License

This project is for educational purposes.

## 👨‍💻 Author

Developed as part of COS20007 - Object Oriented Programming coursework.

## 🐛 Known Issues & Future Enhancements

- [ ] Add mobile-responsive UI
- [ ] Implement attendance reports export
- [ ] Add email notifications
- [ ] Support for multiple attendance sessions per class
- [ ] Student attendance history dashboard
- [ ] Real-time QR code validation feedback

## 📞 Support

For issues or questions, please create an issue in the repository.
