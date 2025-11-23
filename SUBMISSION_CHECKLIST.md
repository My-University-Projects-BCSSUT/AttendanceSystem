# Option 4 Submission Checklist
## COS20007 - Custom Program Assignment

**Student**: Thura Zaw  
**Option**: 4 - Challenging Custom Program (30 marks)  
**Deadline**: Week 13, June 5, 2025

---

## ✅ Week 10 Milestone (COMPULSORY)

### Submission Requirements:
- [ ] **Basic overview of program** (use PROJECT_SUMMARY.md or DESIGN_REPORT_OPTION4.md intro)
- [ ] **Class diagram** (photo or scan) - Available in UML_DIAGRAMS.md
- [ ] **At least one sequence diagram** (photo or scan) - 3 diagrams available in UML_DIAGRAMS.md
- [ ] **List of design patterns you plan to use**:
  - ✅ Factory Pattern (UserFactory)
  - ✅ Builder Pattern (AttendanceSessionBuilder, CourseBuilder)
  - ✅ Singleton Pattern (QRCodeService, ExcelService)

### How to Prepare:
1. Print or screenshot the UML diagrams from `UML_DIAGRAMS.md`
2. Take photos if handwritten, or use the ASCII art diagrams
3. Write 1-2 paragraph overview of your Attendance System
4. List the 3 design patterns with brief explanation

---

## ✅ Week 13 Final Submission

### Required Files:

#### 1. Source Code
- [ ] Complete AttendanceSystem project folder
- [ ] All .cs files (Controllers, Models, Services, Patterns)
- [ ] All .cshtml view files
- [ ] appsettings.json (with placeholder Google credentials)
- [ ] Program.cs with dependency injection setup
- [ ] Database migrations

**Tip**: Zip the entire `AttendanceSystem` folder

#### 2. Final Report
- [ ] **Design Report** (use DESIGN_REPORT_OPTION4.md)
  - [ ] Program overview (what it does, key features)
  - [ ] OOP principles demonstration (abstraction, encapsulation, inheritance, polymorphism)
  - [ ] Design patterns explanation (Factory, Builder, Singleton)
  - [ ] UML class diagram (final version)
  - [ ] Sequence diagrams (at least 2)
  - [ ] Real-world evidence (target users, benefits)
  - [ ] Complexity justification (why it's Option 4 level)
  - [ ] Screenshots of application
  - [ ] Learning reflection

**Suggested Structure**:
1. Executive Summary
2. Program Overview
3. OOP Principles (with code examples)
4. Design Patterns (with code examples and UML)
5. Real-World Value
6. Technical Details
7. Complexity Analysis
8. Conclusion

#### 3. Screenshots
- [ ] Admin Dashboard (showing CRUD operations)
- [ ] Teacher QR Code Display (with session code)
- [ ] Student QR Scanning (dual mode: camera + manual)
- [ ] Attendance Records (grouped by session)
- [ ] Reports/Statistics page
- [ ] Excel Import interface
- [ ] Google Login option

**Organize in folder**: `Screenshots/`

#### 4. Usage Documentation
- [ ] How to run (Docker SQL Server, dotnet run)
- [ ] Default login credentials
- [ ] How to test each feature
- [ ] Sample workflows (Admin, Teacher, Student)

**Use**: README.md (already complete)

---

## ✅ Week 14 Interview Preparation

### Interview Duration: 10-15 minutes

### What to Prepare:

#### 1. Presentation Slides (5-7 slides)
- [ ] **Slide 1**: Title (Your name, student ID, project name)
- [ ] **Slide 2**: Project overview (what it does, who uses it)
- [ ] **Slide 3**: OOP principles demonstration (with code snippets)
- [ ] **Slide 4**: Design Pattern 1 - Factory (diagram + code)
- [ ] **Slide 5**: Design Pattern 2 - Builder (diagram + code)
- [ ] **Slide 6**: Design Pattern 3 - Singleton (diagram + code)
- [ ] **Slide 7**: Demo & Q&A

#### 2. Live Demonstration
Prepare to show:
- [ ] Admin creating a class and enrolling students
- [ ] Teacher starting attendance session (show QR code + session code)
- [ ] Student scanning QR code (camera or manual)
- [ ] Attendance records with statistics
- [ ] Session auto-expiration and absent marking

**Demo Flow** (5 minutes):
1. Login as Admin → Create class → Enroll student
2. Login as Teacher → Start session → Show QR code
3. Login as Student → Scan QR → Success
4. Back to Teacher → View attendance → Close session
5. Show absent student marked automatically

#### 3. Expected Questions & Answers

**Q: Why did you choose these design patterns?**
A: 
- Factory: Needed to create users with different roles (Admin, Teacher, Student) with consistent setup
- Builder: AttendanceSession has many parameters (class, date, QR code, expiration) - builder makes it readable
- Singleton: QRCodeService and ExcelService should be shared across application, thread-safe implementation

**Q: How does your QR code system work?**
A:
- Teacher starts session → SessionBuilder generates UUID and QR code
- QR code contains JSON: `{"type":"attendance","sessionId":20,"code":"uuid"}`
- Student scans → JavaScript parses JSON, extracts code
- Code sent to controller → validates session not expired → creates attendance record
- Session expires after 15 minutes → marks absent students

**Q: What OOP principles did you use?**
A:
- Abstraction: IAttendanceService, IUserFactory interfaces hide implementation
- Encapsulation: Private fields, public properties, Singleton private constructor
- Inheritance: ApplicationUser extends IdentityUser
- Polymorphism: Interface-based services, dependency injection

**Q: Why is this Option 4 level?**
A:
- 3 design patterns properly implemented
- Advanced features: auto-expiration, absent marking, dual scanning
- External integrations: Google OAuth, Excel import
- Complex relationships: 7 entities with navigation properties
- Real-world value: solves actual problem in education
- Production-ready: SQL Server, security, timezone handling

**Q: What was the biggest challenge?**
A:
- QR code JSON parsing - QR contains JSON but controller expects string
- Solution: JavaScript parses JSON and extracts code field
- Session expiration - sessions stayed active forever
- Solution: Auto-check on ViewAttendance, expire and mark absent

**Q: How did you ensure code quality?**
A:
- Separation of concerns: Controllers → Services → Patterns
- Dependency injection for loose coupling
- Error handling with try-catch and TempData messages
- Async/await for database operations
- Input validation on models
- Git version control with meaningful commits

#### 4. Code Examples to Know Well

Be ready to explain these on whiteboard/code:

**Factory Pattern**:
```csharp
var user = await _userFactory.CreateUserAsync(
    email, firstName, lastName, "Teacher", password
);
// Creates user, assigns role, returns ApplicationUser
```

**Builder Pattern**:
```csharp
var session = _sessionBuilder
    .SetClass(classEntity)
    .SetSessionDate(DateTime.UtcNow)
    .SetQRCodeExpiration(15)
    .SetActive(true)
    .Build();
// Generates UUID, creates QR code, sets expiration
```

**Singleton Pattern**:
```csharp
public class QRCodeService
{
    private static QRCodeService? _instance;
    private static readonly object _lock = new object();
    
    private QRCodeService() { } // Private constructor
    
    public static QRCodeService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock) // Thread-safe
                {
                    if (_instance == null)
                        _instance = new QRCodeService();
                }
            }
            return _instance;
        }
    }
}
```

---

## ✅ Pre-Submission Checklist

### Code Quality:
- [ ] All code compiles without errors
- [ ] No hardcoded sensitive data (passwords, API keys)
- [ ] Comments on complex logic
- [ ] Consistent naming conventions
- [ ] No unused imports or variables

### Testing:
- [ ] Admin can create all entities (semesters, courses, classes, teachers, students)
- [ ] Teacher can start session, view QR code, close session
- [ ] Student can scan QR code (camera + manual)
- [ ] Attendance records show correct status (Present/Late/Absent)
- [ ] Session expires after 15 minutes
- [ ] Absent students marked automatically
- [ ] Excel import works
- [ ] Google login works (or clearly disabled if no credentials)

### Documentation:
- [ ] README.md with setup instructions
- [ ] PROJECT_SUMMARY.md with overview
- [ ] DESIGN_REPORT_OPTION4.md (main submission document)
- [ ] UML_DIAGRAMS.md with class and sequence diagrams
- [ ] Comments in code explaining design patterns

### Files to Submit:
```
AttendanceSystem_ThurazZaw_[StudentID].zip
├── AttendanceSystem/           # Complete source code
├── Screenshots/                # Application screenshots
├── DESIGN_REPORT_OPTION4.md    # Main report
├── UML_DIAGRAMS.md             # Diagrams
├── README.md                   # Setup instructions
└── PresentationSlides.pptx     # For interview
```

---

## ✅ Week-by-Week Timeline

### Week 9-10 (NOW):
- [x] Complete implementation (DONE)
- [ ] Create UML diagrams
- [ ] Write Week 10 milestone document
- [ ] **Submit Week 10 milestone**
- [ ] Get feedback from tutor

### Week 11-12:
- [ ] Write final design report
- [ ] Take screenshots
- [ ] Create presentation slides
- [ ] Practice live demo
- [ ] Prepare for interview questions

### Week 13:
- [ ] Final testing
- [ ] **Submit source code and report**
- [ ] Prepare demo for interview

### Week 14:
- [ ] **Attend interview**
- [ ] Present project
- [ ] Answer panel questions

---

## 📊 Marking Criteria (Option 4 - 30 marks)

Based on assignment document:

| Criteria | Weight | How to Excel |
|----------|--------|--------------|
| **Complexity of custom program** | 25% | Advanced features (auto-expiration, dual scanning), 7 entities, external integrations |
| **Number of design patterns** | 20% | 3 patterns properly implemented (Factory, Builder, Singleton) |
| **Correctness of source code** | 20% | No errors, proper implementation, follows best practices |
| **Quality of report** | 20% | Comprehensive DESIGN_REPORT_OPTION4.md with UML, code examples, reflection |
| **Interview performance** | 15% | Clear explanation, live demo, answer questions confidently |

### Tips to Maximize Marks:

**Complexity (7.5/30)**:
- ✅ 7 entities with relationships
- ✅ Advanced features (auto-expiration, absent marking)
- ✅ External integrations (Google OAuth, Excel)
- ✅ Real-world value demonstrated

**Design Patterns (6/30)**:
- ✅ 3 patterns (Factory, Builder, Singleton)
- ✅ Proper implementation (not just naming)
- ✅ Clear UML diagrams
- ✅ Code examples in report

**Code Quality (6/30)**:
- ✅ No compilation errors
- ✅ Proper OOP principles
- ✅ Clean architecture (MVC + Services)
- ✅ Error handling

**Report Quality (6/30)**:
- ✅ Comprehensive DESIGN_REPORT_OPTION4.md
- ✅ UML diagrams (class + sequence)
- ✅ Real-world evidence
- ✅ Learning reflection

**Interview (4.5/30)**:
- ✅ Clear explanation of design patterns
- ✅ Successful live demo
- ✅ Answer questions confidently
- ✅ Show understanding of OOP

---

## 📝 Quick Reference

### Your Design Patterns:

1. **Factory Pattern**: `Patterns/Factory/UserFactory.cs`
   - Creates users with roles (Admin, Teacher, Student)

2. **Builder Pattern**: 
   - `Patterns/Builder/AttendanceSessionBuilder.cs` (sessions with QR codes)
   - `Patterns/Builder/CourseBuilder.cs` (courses)

3. **Singleton Pattern**:
   - `Patterns/Singleton/QRCodeService.cs` (QR generation)
   - `Patterns/Singleton/ExcelService.cs` (Excel operations)

### Your OOP Principles:

- **Abstraction**: `IAttendanceService`, `IUserFactory`, `IAttendanceSessionBuilder`
- **Encapsulation**: Private fields, public properties, Singleton private constructor
- **Inheritance**: `ApplicationUser` extends `IdentityUser`
- **Polymorphism**: Interface-based services, dependency injection

### Your Advanced Features:

- Dual QR scanning (camera + manual)
- JSON QR code format
- Session auto-expiration (15 minutes)
- Automatic absent marking
- Timezone handling (UTC storage, local display)
- Excel bulk import
- Google OAuth integration

---

## 🎯 Final Tips

1. **For Week 10 Milestone**: Keep it brief but complete - show you have a plan
2. **For Report**: Use DESIGN_REPORT_OPTION4.md as template, add your screenshots
3. **For Interview**: Practice explaining design patterns in 2-3 sentences each
4. **For Demo**: Practice the 5-minute demo flow until it's smooth

**You've got this!** Your project is solid, documentation is ready, now just organize and submit! 🚀
