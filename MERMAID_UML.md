# Mermaid UML Diagrams for Attendance System

## Main Class Diagram

Copy and paste this into https://www.mermaidchart.com/

```mermaid
classDiagram
    %% Inheritance
    IdentityUser <|-- ApplicationUser
    ApplicationUser <|-- Admin
    ApplicationUser <|-- Teacher
    ApplicationUser <|-- Student
    
    %% Factory Pattern
    class IUserFactory {
        <<interface>>
        +CreateUserAsync(email, firstName, lastName, role, password) Task~ApplicationUser~
    }
    IUserFactory <|.. UserFactory
    
    %% Builder Pattern
    class IAttendanceSessionBuilder {
        <<interface>>
        +SetClass(class) IAttendanceSessionBuilder
        +SetSessionDate(date) IAttendanceSessionBuilder
        +SetQRCodeExpiration(minutes) IAttendanceSessionBuilder
        +SetActive(isActive) IAttendanceSessionBuilder
        +Build() AttendanceSession
    }
    IAttendanceSessionBuilder <|.. AttendanceSessionBuilder
    
    class ICourseBuilder {
        <<interface>>
        +SetCode(code) ICourseBuilder
        +SetName(name) ICourseBuilder
        +SetDescription(desc) ICourseBuilder
        +SetCredits(credits) ICourseBuilder
        +SetSemester(semester) ICourseBuilder
        +Build() Course
    }
    ICourseBuilder <|.. CourseBuilder
    
    %% User Classes
    class IdentityUser {
        <<Microsoft Identity>>
        +string Id
        +string UserName
        +string Email
    }
    
    class ApplicationUser {
        +string FirstName
        +string LastName
        +DateTime CreatedAt
        +ICollection~Attendance~ Attendances
        +GetFullName() string
    }
    
    class Admin {
        <<Role>>
    }
    
    class Teacher {
        <<Role>>
    }
    
    class Student {
        <<Role>>
    }
    
    %% Factory
    class UserFactory {
        -UserManager~ApplicationUser~ _userManager
        +CreateUserAsync(email, firstName, lastName, role, password) Task~ApplicationUser~
        -GenerateRandomPassword() string
    }
    
    %% Builders
    class AttendanceSessionBuilder {
        -AttendanceSession _session
        -QRCodeService _qrCodeService
        +SetClass(class) IAttendanceSessionBuilder
        +SetSessionDate(date) IAttendanceSessionBuilder
        +SetQRCodeExpiration(minutes) IAttendanceSessionBuilder
        +SetActive(isActive) IAttendanceSessionBuilder
        +Build() AttendanceSession
    }
    
    class CourseBuilder {
        -Course _course
        +SetCode(code) ICourseBuilder
        +SetName(name) ICourseBuilder
        +SetDescription(desc) ICourseBuilder
        +SetCredits(credits) ICourseBuilder
        +SetSemester(semester) ICourseBuilder
        +Build() Course
    }
    
    %% Singleton Pattern
    class QRCodeService {
        <<Singleton>>
        -QRCodeService _instance$
        -object _lock$
        -QRCodeService()
        +Instance$ QRCodeService
        +GenerateAttendanceQRData(sessionId, code) string
        +GenerateQRCodeBase64(data) string
    }
    
    class ExcelService {
        <<Singleton>>
        -ExcelService _instance$
        -object _lock$
        -ExcelService()
        +Instance$ ExcelService
        +ReadExcelAsync(filePath) Task~List~StudentImportDto~~
        +ExportToExcelAsync(data, filePath) Task
    }
    
    %% Domain Models
    class Semester {
        +int Id
        +string Name
        +DateTime StartDate
        +DateTime EndDate
        +ICollection~Course~ Courses
    }
    
    class Course {
        +int Id
        +string Code
        +string Name
        +string Description
        +int Credits
        +int SemesterId
        +Semester Semester
        +ICollection~Class~ Classes
    }
    
    class Class {
        +int Id
        +string Name
        +int CourseId
        +string TeacherId
        +DayOfWeek DayOfWeek
        +TimeSpan StartTime
        +TimeSpan EndTime
        +Course Course
        +ApplicationUser Teacher
        +ICollection~ClassEnrollment~ Enrollments
        +ICollection~AttendanceSession~ AttendanceSessions
    }
    
    class ClassEnrollment {
        +int Id
        +int ClassId
        +string StudentId
        +DateTime EnrolledAt
        +Class Class
        +ApplicationUser Student
    }
    
    class AttendanceSession {
        +int Id
        +int ClassId
        +DateTime SessionDate
        +string QRCode
        +DateTime QRCodeExpiresAt
        +bool IsActive
        +DateTime CreatedAt
        +Class Class
        +ICollection~Attendance~ Attendances
        +IsQRCodeValid() bool
    }
    
    class Attendance {
        +int Id
        +int AttendanceSessionId
        +string StudentId
        +AttendanceStatus Status
        +DateTime? CheckInTime
        +string Notes
        +DateTime CreatedAt
        +AttendanceSession AttendanceSession
        +ApplicationUser Student
    }
    
    class AttendanceStatus {
        <<enumeration>>
        Present
        Late
        Absent
        Excused
    }
    
    %% Services
    class IAttendanceService {
        <<interface>>
        +CreateSessionAsync(classId, expirationMinutes) Task~AttendanceSession~
        +SubmitAttendanceAsync(sessionId, studentId, qrCode) Task~Attendance~
        +GetClassAttendanceAsync(classId) Task~List~Attendance~~
        +GetActiveSessionByQRCodeAsync(qrCode) Task~AttendanceSession~
        +ExpireSessionAsync(sessionId) Task
        +MarkAbsentStudentsAsync(sessionId) Task
    }
    
    class AttendanceService {
        -ApplicationDbContext _context
        -IAttendanceSessionBuilder _sessionBuilder
        -QRCodeService _qrCodeService
        +CreateSessionAsync(classId, expirationMinutes) Task~AttendanceSession~
        +SubmitAttendanceAsync(sessionId, studentId, qrCode) Task~Attendance~
        +GetClassAttendanceAsync(classId) Task~List~Attendance~~
        +GetActiveSessionByQRCodeAsync(qrCode) Task~AttendanceSession~
        +ExpireSessionAsync(sessionId) Task
        +MarkAbsentStudentsAsync(sessionId) Task
    }
    
    IAttendanceService <|.. AttendanceService
    
    %% Relationships
    Semester "1" --> "*" Course : contains
    Course "1" --> "*" Class : has
    Class "1" --> "*" ClassEnrollment : enrolls
    Class "1" --> "*" AttendanceSession : creates
    AttendanceSession "1" --> "*" Attendance : records
    ApplicationUser "1" --> "*" Attendance : submits
    ApplicationUser "1" --> "*" ClassEnrollment : enrolls in
    ApplicationUser "1" --> "*" Class : teaches
    
    AttendanceSessionBuilder --> QRCodeService : uses
    AttendanceService --> QRCodeService : uses
    AttendanceService --> IAttendanceSessionBuilder : uses
    Attendance --> AttendanceStatus : has
```

---

## Simplified Class Diagram (Core Components Only)

If the above is too complex, use this simplified version:

```mermaid
classDiagram
    %% Design Patterns
    class IUserFactory {
        <<interface>>
        +CreateUserAsync() Task~ApplicationUser~
    }
    class UserFactory {
        +CreateUserAsync() Task~ApplicationUser~
    }
    IUserFactory <|.. UserFactory
    
    class IAttendanceSessionBuilder {
        <<interface>>
        +SetClass() IAttendanceSessionBuilder
        +SetSessionDate() IAttendanceSessionBuilder
        +Build() AttendanceSession
    }
    class AttendanceSessionBuilder {
        +SetClass() IAttendanceSessionBuilder
        +Build() AttendanceSession
    }
    IAttendanceSessionBuilder <|.. AttendanceSessionBuilder
    
    class QRCodeService {
        <<Singleton>>
        -_instance$ QRCodeService
        +Instance$ QRCodeService
        +GenerateQRCodeBase64() string
    }
    
    %% Domain Models
    class ApplicationUser {
        +string Id
        +string FirstName
        +string LastName
        +string Email
    }
    
    class Semester {
        +int Id
        +string Name
        +DateTime StartDate
        +DateTime EndDate
    }
    
    class Course {
        +int Id
        +string Code
        +string Name
        +int Credits
    }
    
    class Class {
        +int Id
        +string Name
        +DayOfWeek DayOfWeek
        +TimeSpan StartTime
    }
    
    class AttendanceSession {
        +int Id
        +DateTime SessionDate
        +string QRCode
        +DateTime QRCodeExpiresAt
        +bool IsActive
    }
    
    class Attendance {
        +int Id
        +AttendanceStatus Status
        +DateTime CheckInTime
    }
    
    %% Relationships
    Semester "1" --> "*" Course
    Course "1" --> "*" Class
    Class "1" --> "*" AttendanceSession
    AttendanceSession "1" --> "*" Attendance
    ApplicationUser "1" --> "*" Attendance
    
    AttendanceSessionBuilder --> QRCodeService
```

---

## Sequence Diagram 1: Start Attendance Session

```mermaid
sequenceDiagram
    participant T as Teacher
    participant TC as TeacherController
    participant AS as AttendanceService
    participant SB as SessionBuilder
    participant QR as QRCodeService
    participant DB as Database
    
    T->>TC: StartSession(classId)
    TC->>AS: CreateSessionAsync(classId, 15)
    AS->>SB: SetClass(classEntity)
    AS->>SB: SetSessionDate(DateTime.UtcNow)
    AS->>SB: SetQRCodeExpiration(15)
    AS->>SB: SetActive(true)
    AS->>SB: Build()
    SB->>QR: Instance (get singleton)
    QR-->>SB: QRCodeService instance
    SB->>QR: GenerateQRCode(uuid)
    QR-->>SB: QR code string
    SB-->>AS: AttendanceSession
    AS->>DB: Save session
    DB-->>AS: Saved
    AS-->>TC: Session with QR code
    TC-->>T: ShowQRCode view (15 min timer)
```

---

## Sequence Diagram 2: Submit Attendance (QR Scan)

```mermaid
sequenceDiagram
    participant S as Student
    participant SC as StudentController
    participant AS as AttendanceService
    participant DB as Database
    
    S->>SC: Scan QR Code (JSON)
    Note over SC: Parse JSON<br/>Extract code
    SC->>AS: SubmitAttendanceAsync(sessionId, studentId, code)
    AS->>DB: GetActiveSessionByQRCode(code)
    DB-->>AS: AttendanceSession
    AS->>AS: Validate: IsQRCodeValid()<br/>(check 15min expiration)
    AS->>DB: Check enrollment
    DB-->>AS: Student enrolled
    AS->>DB: Check if already submitted
    DB-->>AS: Not submitted yet
    AS->>AS: Calculate status<br/>(Present/Late)
    AS->>DB: Create Attendance record
    DB-->>AS: Saved
    AS-->>SC: Success
    SC-->>S: AttendanceSuccess page
```

---

## Sequence Diagram 3: Auto-Expire Session

```mermaid
sequenceDiagram
    participant T as Teacher
    participant TC as TeacherController
    participant AS as AttendanceService
    participant DB as Database
    
    T->>TC: ViewAttendance(classId)
    TC->>DB: Get expired sessions<br/>(QRCodeExpiresAt < Now)
    DB-->>TC: List of expired sessions
    
    loop For each expired session
        TC->>AS: ExpireSessionAsync(sessionId)
        AS->>DB: Set IsActive = false
        DB-->>AS: Updated
        
        TC->>AS: MarkAbsentStudentsAsync(sessionId)
        AS->>DB: Get enrolled students
        DB-->>AS: Student list
        AS->>DB: Get students with attendance
        DB-->>AS: Attended list
        Note over AS: Calculate:<br/>Enrolled - Attended
        AS->>DB: Create Absent records<br/>(Status = Absent)
        DB-->>AS: Saved
    end
    
    TC->>DB: Get all attendance records
    DB-->>TC: Attendance list
    TC-->>T: ViewAttendance page<br/>(grouped by session)
```

---

## Usage Instructions

### For Mermaid Chart (https://www.mermaidchart.com/):

1. Copy the code block (starting with `classDiagram` or `sequenceDiagram`)
2. Paste into the Mermaid Chart editor
3. The diagram will render automatically
4. Export as PNG/SVG for your report

### Tips:

- **Main Class Diagram**: Shows all design patterns and relationships (comprehensive)
- **Simplified Class Diagram**: Shows core components only (easier to read)
- **Sequence Diagrams**: Show the flow of operations

### For Your Report:

Include:
1. **Main Class Diagram** - to show complete architecture
2. **Sequence Diagram 1** - to show Builder and Singleton patterns in action
3. **Sequence Diagram 2** - to show student attendance flow
4. **Sequence Diagram 3** - to show auto-expiration feature

You can also export these as images and include them in your DESIGN_REPORT_OPTION4.md!
