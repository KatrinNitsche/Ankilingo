using AnkiLingoExcelService.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnkiLingo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // existing sets
        public DbSet<UserData> UserData { get; set; }
        public DbSet<UserCourseData> UserCourseData { get; set; }

        // Add these so EF Core tracks course-related entities
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<ImageData> Images { get; set; }
        public DbSet<ImageWord> ImageWords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // optional: configure cascade deletes / relationships and key generation if needed
            // builder.Entity<Course>().Property(c => c.Id).ValueGeneratedOnAdd();
        }
    }
}
