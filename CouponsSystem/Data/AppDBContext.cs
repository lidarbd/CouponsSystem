using Microsoft.EntityFrameworkCore;

namespace CouponsSystem.Data
{
    public class AppDBContext : DbContext
    {

        public AppDBContext() { }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        
    }
}
