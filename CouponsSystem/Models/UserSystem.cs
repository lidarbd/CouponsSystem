namespace CouponsSystem.Models
{
    public class UserSystem
    {
        private Dictionary<int, AdminUser> adminUsers = new Dictionary<int, AdminUser>();
        private HashSet<string> loggedInUserIDs = new HashSet<string>();

        public void CreateAdminUser(int id, string password, string username)
        {
            AdminUser newAdmin = new AdminUser(id, password, username);
            addAdminUser(id, newAdmin);
        }

        public bool AuthenticateAdminUser(int id, string password)
        {
            AdminUser user = GetAdminUser(id);
            if (user != null)
            {
                return user.VerifyPassword(password);
            }
            return false;
        }


        // Returns true if logged in successfully
        public bool LogInUser(string id)
        {
            return loggedInUserIDs.Add(id);
        }

        private bool isUserLoggedIn(string id)
        {
            return loggedInUserIDs.Contains(id);
        }

        // Returns true if logged out successfully
        public bool LogOutUser(string id)
        {
            return loggedInUserIDs.Remove(id);
        }

        private void addAdminUser(int id, AdminUser user)
        {
            if (!adminUsers.ContainsKey(id))
            {
                adminUsers[id] = user;
            }
        }

        public AdminUser GetAdminUser(int id)
        {
            if (adminUsers.TryGetValue(id, out AdminUser user))
            {
                return user;
            }
            return null;
        }
    }
}
