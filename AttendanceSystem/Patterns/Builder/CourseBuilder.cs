using AttendanceSystem.Models;

namespace AttendanceSystem.Patterns.Builder
{
    // Builder Pattern for Course creation with validation
    public interface ICourseBuilder
    {
        ICourseBuilder SetCode(string code);
        ICourseBuilder SetName(string name);
        ICourseBuilder SetDescription(string description);
        ICourseBuilder SetCredits(int credits);
        ICourseBuilder SetSemester(Semester semester);
        Course Build();
        void Reset();
    }

    public class CourseBuilder : ICourseBuilder
    {
        private Course _course = null!;

        public CourseBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _course = new Course
            {
                CreatedAt = DateTime.UtcNow
            };
        }

        public ICourseBuilder SetCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Course code cannot be empty.");
            
            _course.Code = code.ToUpper();
            return this;
        }

        public ICourseBuilder SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Course name cannot be empty.");
            
            _course.Name = name;
            return this;
        }

        public ICourseBuilder SetDescription(string description)
        {
            _course.Description = description ?? string.Empty;
            return this;
        }

        public ICourseBuilder SetCredits(int credits)
        {
            if (credits <= 0)
                throw new ArgumentException("Credits must be greater than 0.");
            
            _course.Credits = credits;
            return this;
        }

        public ICourseBuilder SetSemester(Semester semester)
        {
            if (semester == null)
                throw new ArgumentException("Semester cannot be null.");
            
            _course.SemesterId = semester.Id;
            _course.Semester = semester;
            return this;
        }

        public Course Build()
        {
            // Validate before building
            if (string.IsNullOrEmpty(_course.Code))
                throw new InvalidOperationException("Course code is required.");
            if (string.IsNullOrEmpty(_course.Name))
                throw new InvalidOperationException("Course name is required.");
            if (_course.SemesterId == 0)
                throw new InvalidOperationException("Semester is required.");
            
            var result = _course;
            Reset();
            return result;
        }
    }
}
