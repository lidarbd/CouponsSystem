namespace CouponsSystem.Models
{
    public class UserSystem
    {
        private Dictionary<int, AdminUsers> adminUsers = new Dictionary<int, AdminUsers>();
        private HashSet<string> loggedInUserIDs = new HashSet<string>();

        public void CreateAdminUser(int id, string password, string username)
        {
            AdminUsers newAdmin = new AdminUsers(id, password, username);
            addAdminUser(id, newAdmin);
        }

        public bool AuthenticateAdminUser(int id, string password)
        {
            AdminUsers user = GetAdminUser(id);
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

        private void addAdminUser(int id, AdminUsers user)
        {
            if (!adminUsers.ContainsKey(id))
            {
                adminUsers[id] = user;
            }
        }

        public AdminUsers GetAdminUser(int id)
        {
            if (adminUsers.TryGetValue(id, out AdminUsers user))
            {
                return user;
            }
            return null;
        }
    }
}
