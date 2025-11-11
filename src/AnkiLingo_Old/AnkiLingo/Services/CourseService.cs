using AnkiLingo.Data;
using AnkiLingo.Repositories;

namespace AnkiLingo.Services
{
    public class CourseService : ICourseService
    {
        private CourseRepository courseRepository;
       
        public CourseService(DataContext context)
        {
            courseRepository = new CourseRepository(context);
        }

        public async Task<Course> Add(Course entry)
        {
            // check if the course already exists
            var existingCourse = courseRepository.GetAll().FirstOrDefault(c => c.Name == entry.Name);
            if (existingCourse != null)
            {
                // If the course exists, update it instead of adding a new one
                existingCourse.Description = entry.Description;
                existingCourse.Icon = entry.Icon;
                existingCourse.Updated = DateTime.Now;
                existingCourse.Sections = entry.Sections;
                return courseRepository.Update(existingCourse);
            }
            else
            {
                courseRepository.Add(entry);
                courseRepository.Commit();
                return entry;
            }            
        }

        public async Task<IEnumerable<Course>> GetAll()
        {
            return courseRepository.GetAll();
        }

        public async Task<Course> GetById(int id)
        {
            return courseRepository.GetById(id);
        }

        public async Task<bool> Remove(int id)
        {
            courseRepository.Remove(id);
            courseRepository.Commit();
            return true;
        }

        public async Task<Course> Update(Course entry)
        {
            var entryToUpdate = await GetById(entry.Id);
            entry.Updated = DateTime.Now;
            entryToUpdate.Name = entry.Name;
            entryToUpdate.Description = entry.Description;
            entryToUpdate.Icon = entry.Icon;

            return courseRepository.Update(entryToUpdate);
        }
    }
}
