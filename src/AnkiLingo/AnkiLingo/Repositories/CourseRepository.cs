using AnkiLingo.Data;
using Microsoft.EntityFrameworkCore;

namespace AnkiLingo.Repositories
{
    public class CourseRepository : BaseRepository<Course>
    {
        public CourseRepository(DataContext context) : base(context) { }
        public override IEnumerable<Course> GetAll() => Context.Courses;
        public override Course? GetById(int id) => Context.Courses
                                                          .Include(x => x.Sections)
                                                          .ThenInclude(x => x.Units)
                                                          .ThenInclude(x => x.Entries)
                                                          .FirstOrDefault(x => x.Id == id);
    }
}
