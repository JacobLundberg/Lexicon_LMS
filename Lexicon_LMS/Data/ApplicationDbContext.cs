using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_LMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserCourse>()
                .HasKey(t => new { t.ApplicationUserId, t.CourseId });
            modelBuilder.Entity<Course>()
                .HasIndex(b => b.Id)
                .IsUnique();
        }

        public DbSet<Course> Course { get; set; }

        public DbSet<ApplicationUserCourse> UserCourse { get; set; }

        public DbSet<Module> Module { get; set; }

        public DbSet<ActivityModel> ActivityModel { get; set; }

        public DbSet<ActivityType> ActivityType { get; set; }

    }
}
