using CouponsSystem.Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CouponsSystem.Models
{
    public class UserSystem
    {
        private readonly AppDbContext _context;

        public UserSystem(AppDbContext context)
        {
            _context = context;
        }

        // Create a new AdminUser and save it to the database
        public async Task CreateAdminUserAsync(string username, string password)
        {
            var newAdmin = new AdminUser(username, password);
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
            bool isValidUserInfo = await authenticateAdminUserAsync(username, password);

            if (isValidUserInfo)
            {
                var user = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    var loggedInUser = new LoggedInUser
                    {
                        UserId = user.Id,
                    };
                    _context.LoggedInUsers.Add(loggedInUser);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            return false; // Login failed
        }

        // Log out a user by removing their id from the LoggedInUsers table
        public async Task<bool> LogOutUser(int userId)
        {
            var session = await _context.LoggedInUsers.FirstOrDefaultAsync(s => s.UserId == userId);
            if (session != null)
            {
                _context.LoggedInUsers.Remove(session);
                await _context.SaveChangesAsync();
                return true;
            }
            return false; // User not found
        }

        // Retrieve an AdminUser by ID from the database
        public async Task<AdminUser?> GetAdminUserAsync(int id)
        {
            return await _context.AdminUsers.FindAsync(id);
        }
    }
}
