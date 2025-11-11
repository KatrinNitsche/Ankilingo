using AnkiLingo.Data;

namespace AnkiLingo.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAll();
        Task<Course> Add(Course entry);
        Task<Course> GetById(int idNumber);
        Task<Course> Update(Course entry);
        Task<bool> Remove(int id);
    }
}
