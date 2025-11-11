using AnkiLingo.Data;

namespace AnkiLingo.Services
{
    public interface ISectionService
    {
        Task<IEnumerable<Section>> GetAll();
        Task<Section> Add(Section entry);
        Task<Section> GetById(int idNumber);
        Task<Section> Update(Section entry);
        Task<bool> Remove(int id);
    }
}
