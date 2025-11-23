# Database Configuration - SQL Server

## ✅ Configuration Complete

My AttendanceSystem is now configured to use **SQL Server (MSSQL)** running in Docker with **Entity Framework Code First** approach.

## Connection Details

- **Host:** localhost (127.0.0.1)
- **Port:** 1433
- **Database:** AttendanceSystemDb
- **Username:** sa
- **Password:** Admin@123

## Connection String

```
Server=localhost,1433;Database=AttendanceSystemDb;User Id=sa;Password=Admin@123;TrustServerCertificate=True;Encrypt=False;
```

Location: `appsettings.json` → ConnectionStrings → DefaultConnection

## Database Schema Created

The following tables were created using EF Core migrations:

### Identity Tables (ASP.NET Core Identity)
- ✅ AspNetUsers
- ✅ AspNetRoles
- ✅ AspNetUserRoles
- ✅ AspNetUserClaims
- ✅ AspNetUserLogins
- ✅ AspNetUserTokens
- ✅ AspNetRoleClaims

### Application Tables
- ✅ Semesters
- ✅ Courses
- ✅ Classes
- ✅ ClassEnrollments
- ✅ AttendanceSessions
- ✅ Attendances

### Relationships Configured
- ✅ Semester → Course (One-to-Many)
- ✅ Course → Class (One-to-Many)
- ✅ Teacher → Class (One-to-Many)
- ✅ Class → ClassEnrollment (One-to-Many)
- ✅ Student → ClassEnrollment (One-to-Many)
- ✅ Class → AttendanceSession (One-to-Many)
- ✅ AttendanceSession → Attendance (One-to-Many)
- ✅ Student → Attendance (One-to-Many)

### Constraints
- ✅ Unique: Course Code + SemesterId
- ✅ Unique: ClassId + StudentId (enrollment)
- ✅ Unique: AttendanceSession QRCode
- ✅ Foreign keys with proper delete behaviors

## Migration Files

Location: `Migrations/20251122111653_InitialCreate.cs`

This migration contains:
- All table creation scripts
- Column definitions with proper data types
- Indexes for performance
- Foreign key constraints
- Unique constraints

## Code First Approach

### What is Code First?
- **Define Models First:** You write C# classes (Models)
- **Generate Database:** EF Core creates database tables from your models
- **Migrations:** Track changes to your models and apply them to database

### Models in This Project
```
Models/
├── User.cs (ApplicationUser extends IdentityUser)
├── Semester.cs
├── Course.cs
├── Class.cs
├── ClassEnrollment.cs
├── AttendanceSession.cs
└── Attendance.cs
```

### DbContext Configuration
Location: `Data/ApplicationDbContext.cs`

Configures:
- DbSets for each entity
- Relationships using Fluent API
- Delete behaviors
- Unique constraints
- Indexes

## Entity Framework Commands Used

```bash
# 1. Create migration
dotnet ef migrations add InitialCreate

# 2. Apply migration (creates database & tables)
dotnet ef database update
```

## Future Database Changes

When you modify models:

```bash
# 1. Create new migration
dotnet ef migrations add DescriptionOfChange

# 2. Apply to database
dotnet ef database update

# 3. Rollback (if needed)
dotnet ef database update PreviousMigrationName
```

## Docker Container

Your SQL Server is running in Docker:

```bash
# Container details
Container ID: ba221fc57446
Image: mcr.microsoft.com/azure-sql-edge
Port: 1433:1433
Name: azuresqledge

# Useful commands
docker ps                    # Check if running
docker stop azuresqledge    # Stop container
docker start azuresqledge   # Start container
docker logs azuresqledge    # View logs
```

## Database Seed Data

When you run the application, `DbInitializer.InitializeAsync()` will:
- Create default roles (Admin, Teacher, Student)
- Create admin user: admin@attendance.com / Admin@123
- Seed initial data if needed

## Verify Database

You can connect to the database using:
- **SQL Server Management Studio (SSMS)**
- **Azure Data Studio**
- **VS Code SQL Server Extension**
- **DBeaver**

Connection details:
```
Server: localhost,1433
Authentication: SQL Server Authentication
Login: sa
Password: Admin@123
Database: AttendanceSystemDb
```

## Package References

The project uses these EF Core packages:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.0" />
```

## Program.cs Configuration

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

Uses `UseSqlServer()` instead of `UseSqlite()`.

## Status

✅ Database created successfully
✅ All tables created
✅ Relationships configured
✅ Constraints applied
✅ Indexes created
✅ Build successful

Your attendance system is ready to run with SQL Server!

## Run the Application

```bash
cd AttendanceSystem
dotnet run
```

Visit: https://localhost:5001
Login: admin@attendance.com / Admin@123

---

**Database Type:** Code First with Entity Framework Core
**RDBMS:** Microsoft SQL Server (Azure SQL Edge in Docker)
**ORM:** Entity Framework Core 9.0
