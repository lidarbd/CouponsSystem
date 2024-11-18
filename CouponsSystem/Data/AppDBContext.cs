using Microsoft.EntityFrameworkCore;
using CouponsSystem.Models;

namespace CouponsSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Coupons> Coupons { get; set; } = null!;
        public DbSet<AdminUsers> AdminUsers { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=127.0.0.1;Database=MySql@localhost:3308;User=root;Password=Lb@210599;SslMode=None");
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Coupons>()
        //        .HasKey(c => c.Code);

        //    modelBuilder.Entity<AdminUsers>()
        //        .HasKey(u => u.Id);
        //}
    }
}
