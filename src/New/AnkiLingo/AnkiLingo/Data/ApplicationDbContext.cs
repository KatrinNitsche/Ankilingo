using AnkiLingoExcelService.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnkiLingo.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<UserData> UserData { get; set; }
        public DbSet<UserCourseData> UserCourseData { get; set; }
    }
}
