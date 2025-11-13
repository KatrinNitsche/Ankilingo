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
        Course GetCourseById(int id);
        Course GetCourseByName(string courseName);
        void AddCourse(Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(int id);
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

        public Course GetCourseById(int id)
        {
            return _dbContext.Courses.Find(id);
        }

        public Course GetCourseByName(string courseName)
        {
            if (string.IsNullOrWhiteSpace(courseName))
                return null;

            // Normalize the search term once on the client; EF will translate c.Name.ToUpper() to SQL UPPER(c.Name).
            var normalized = courseName.ToUpper();
            return _dbContext.Courses.FirstOrDefault(c => c.Name != null && c.Name.ToUpper() == normalized);
        }

        public void AddCourse(Course course)
        {
            _dbContext.Courses.Add(course);
            _dbContext.SaveChanges();
        }

        public void UpdateCourse(Course course)
        {
            var existingCourse = GetCourseById(course.Id);
            if (existingCourse != null)
            {
                _dbContext.Entry(existingCourse).CurrentValues.SetValues(course);
                _dbContext.SaveChanges();
            }
        }

        public void DeleteCourse(int id)
        {
            var course = GetCourseById(id);
            if (course != null)
            {
                _dbContext.Courses.Remove(course);
                _dbContext.SaveChanges();
            }
        }
    }
}
