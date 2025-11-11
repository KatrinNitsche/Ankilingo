using AnkiLingo.Data;

namespace AnkiLingo.Services
{
    public interface IUnitService
    {
        Task<IEnumerable<Unit>> GetAll();
        Task<Unit> Add(Unit entry);
        Task<Unit> GetById(int idNumber);
        Task<Unit> Update(Unit entry);
        Task<bool> Remove(int id);
    }
}
