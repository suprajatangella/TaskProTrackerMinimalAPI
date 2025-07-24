using Microsoft.EntityFrameworkCore;
using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            System.Diagnostics.Debugger.Launch();
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project) 
            .WithMany(p=>p.Tasks);
        }

    }
}
