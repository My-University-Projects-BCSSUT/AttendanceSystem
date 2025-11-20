using AttendanceSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace AttendanceSystem.Patterns.Factory
{
    // Factory Pattern for User Creation
    public interface IUserFactory
    {
        Task<ApplicationUser> CreateUserAsync(string email, string firstName, string lastName, string role);
    }

    public class UserFactory : IUserFactory
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> CreateUserAsync(string email, string firstName, string lastName, string role)
        {
            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, GenerateDefaultPassword());
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return user;
            }

            throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        private string GenerateDefaultPassword()
        {
            return "TempPass123!"; // In production, this should be randomly generated and emailed
        }
    }
}
