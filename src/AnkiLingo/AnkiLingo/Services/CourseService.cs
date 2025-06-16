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
            courseRepository.Add(entry);
            courseRepository.Commit();
            return entry;
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
