using CouponsSystem.Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CouponsSystem.Models
{
    public class UserSystem
    {
        private readonly AppDbContext _context;
        private HashSet<string> _loggedInUserIds = new HashSet<string>();

        public UserSystem(AppDbContext context)
        {
            _context = context;
        }

        // Create a new AdminUser and save it to the database
        public async Task CreateAdminUserAsync(int id, string username, string password)
        {
            var newAdmin = new AdminUser(id, username, password);
            _context.AdminUsers.Add(newAdmin);
            await _context.SaveChangesAsync();
        }

        // Authenticate an AdminUser by verifying their password and username
        private async Task<bool> authenticateAdminUserAsync(string username, string password)
        {
            var user = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);

            // Check if user exists and the password is correct
            return user != null && user.VerifyPassword(password);
        }

        // Log in a user by adding their ID to the logged-in list
        public async Task<bool> LogInUserAsync(string username, string password)
        {
            // Call AuthenticateAdminUserAsync to validate user credentials
            bool isValidUserInfo = await authenticateAdminUserAsync(username, password);

            if (isValidUserInfo)
            {
                // Find the user again to get their ID
                var user = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);

                // Add the user ID to the logged-in list
                return user != null && _loggedInUserIds.Add(user.Id.ToString());
            }

            return false; // Login failed
        }

        // Check if a user is already logged in
        public bool IsUserLoggedIn(string id)
        {
            return _loggedInUserIds.Contains(id);
        }

        // Log out a user by removing their ID from the logged-in list
        public bool LogOutUser(string id)
        {
            return _loggedInUserIds.Remove(id);
        }

        // Retrieve an AdminUser by ID from the database
        public async Task<AdminUser?> GetAdminUserAsync(int id)
        {
            return await _context.AdminUsers.FindAsync(id);
        }
    }
}
