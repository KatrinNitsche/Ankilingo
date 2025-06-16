using AnkiLingo.Data;

namespace AnkiLingo.Services
{
    public interface IEntryService
    {
        Task<IEnumerable<Entry>> GetAll();
        Task<Entry> Add(Entry entry);
        Task<Entry> GetById(int idNumber);
        Task<Entry> Update(Entry entry);
        Task<bool> Remove(int id);
    }
}
