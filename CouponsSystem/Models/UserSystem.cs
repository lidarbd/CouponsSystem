using CouponsSystem.Data;
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

        // Authenticate an AdminUser by verifying their password
        public async Task<bool> AuthenticateAdminUserAsync(int id, string password)
        {
            var user = await _context.AdminUsers.FindAsync(id);
            return user != null && user.VerifyPassword(password);
        }
        

        // Log in a user by adding their ID to the logged-in list
        public bool LogInUser(string id)
        {
            return _loggedInUserIds.Add(id);
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
