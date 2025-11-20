using AttendanceSystem.Models;

namespace AttendanceSystem.Patterns.Builder
{
    // Builder Pattern for complex AttendanceSession creation
    public interface IAttendanceSessionBuilder
    {
        IAttendanceSessionBuilder SetClass(Class classEntity);
        IAttendanceSessionBuilder SetSessionDate(DateTime sessionDate);
        IAttendanceSessionBuilder SetQRCodeExpiration(int expirationMinutes);
        IAttendanceSessionBuilder SetActive(bool isActive);
        AttendanceSession Build();
        void Reset();
    }

    public class AttendanceSessionBuilder : IAttendanceSessionBuilder
    {
        private AttendanceSession _session = null!;

        public AttendanceSessionBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _session = new AttendanceSession
            {
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public IAttendanceSessionBuilder SetClass(Class classEntity)
        {
            _session.ClassId = classEntity.Id;
            _session.Class = classEntity;
            return this;
        }

        public IAttendanceSessionBuilder SetSessionDate(DateTime sessionDate)
        {
            _session.SessionDate = sessionDate;
            return this;
        }

        public IAttendanceSessionBuilder SetQRCodeExpiration(int expirationMinutes)
        {
            _session.QRCode = Guid.NewGuid().ToString();
            _session.QRCodeExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            return this;
        }

        public IAttendanceSessionBuilder SetActive(bool isActive)
        {
            _session.IsActive = isActive;
            return this;
        }

        public AttendanceSession Build()
        {
            var result = _session;
            Reset(); // Prepare for next build
            return result;
        }
    }
}
