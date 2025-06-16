using Microsoft.EntityFrameworkCore;

namespace AnkiLingo.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Entry> Entries { get; set; }
    }
}
