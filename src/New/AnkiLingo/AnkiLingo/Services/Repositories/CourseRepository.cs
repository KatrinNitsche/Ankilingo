using AnkiLingo.Data;
using Microsoft.EntityFrameworkCore;

namespace AnkiLingo.Services.Repositories
{
    /// <summary>
    /// Interface for course repository (CRUD operations for courses).
    /// </summary>
    public interface ICourseRepository
    {
        IEnumerable<Course> GetAllCourses();
        Task<IEnumerable<string>> GetCourseNamesAsync();
        Task<Course> GetCourseById(int id);
        Task<Course> GetCourseByName(string courseName);
        Task<bool> AddCourse(Course course);
        Task<bool> UpdateCourse(Course course);
        Task<bool> DeleteCourse(int id);
    }

    public class CourseRepository : ICourseRepository
    {
        private ApplicationDbContext _dbContext;

        public CourseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Course> GetAllCourses()
        {
            return _dbContext.Courses.ToList();
        }

        public async Task<IEnumerable<string>> GetCourseNamesAsync()
        {
            var data = await _dbContext.Courses.Select(c => c.Name).ToListAsync();
            return data;
        }

        public async Task<Course> GetCourseById(int id)
        {
            return await _dbContext.Courses.FindAsync(id);
        }

        public async Task<Course> GetCourseByName(string courseName)
        {
            if (string.IsNullOrWhiteSpace(courseName)) return null;
            return _dbContext.Courses.Include(x => x.Sections)
                .ThenInclude(x => x.Units)
                .ThenInclude(x => x.Entries)
                .FirstOrDefault(c => c.Name != null && c.Name == courseName);
        }

        public async Task<bool> AddCourse(Course course)
        {
            _dbContext.Courses.Add(course);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCourse(Course course)
        {
            var existingCourse = await GetCourseById(course.Id);
            if (existingCourse != null)
            {
                _dbContext.Entry(existingCourse).CurrentValues.SetValues(course);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteCourse(int id)
        {
            var course = await GetCourseById(id);
            if (course != null)
            {
                _dbContext.Courses.Remove(course);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
