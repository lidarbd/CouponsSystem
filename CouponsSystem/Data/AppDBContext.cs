using Microsoft.EntityFrameworkCore;
using CouponsSystem.Models;

namespace CouponsSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Coupon> Coupons { get; set; } = null!;
        public DbSet<AdminUser> AdminUsers { get; set; } = null!;
        public DbSet<LoggedInUser> LoggedInUsers { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}
