using AnkiLingo.Services.Repositories;
using AnkiLingo.Data;

namespace AnkiLingo.Services
{
    public interface ICourseService
    {
        IEnumerable<Course> GetAllCourses();
        Task<Course> GetCourseById(int id);
        bool AddCourse(Course course);
        bool UpdateCourse(Course course);
        bool DeleteCourse(int id);
    }

    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ICourseRepository courseRepository, ILogger<CourseService> logger)
        {
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public IEnumerable<Course> GetAllCourses()
        {
            return _courseRepository.GetAllCourses();
        }

        public async Task<Course> GetCourseById(int id)
        {
            return await _courseRepository.GetCourseById(id);
        }
        public bool AddCourse(Course course)
        {
            try
            {
                // check if course already exists
                if (_courseRepository.GetCourseByName(course.Name) != null)
                {
                    _logger.LogWarning("Course with name {CourseName} already exists", course.Name);
                    return false;
                }

                _courseRepository.AddCourse(course);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding course");
                return false;
            }
        }

        public bool UpdateCourse(Course course)
        {
            try
            {
                _courseRepository.UpdateCourse(course);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course");
                return false;
            }
        }

        public bool DeleteCourse(int id)
        {
            try
            {
                _courseRepository.DeleteCourse(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course");
                return false;
            }
        }
    }
}
