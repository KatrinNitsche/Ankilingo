using AnkiLingo.Data;
using AnkiLingo.Repositories;

namespace AnkiLingo.Services
{
    public class UnitService : IUnitService
    {
        private UnitRepository UnitRepository;

        public UnitService(DataContext context)
        {
            UnitRepository = new UnitRepository(context);
        }

        public async Task<Unit> Add(Unit entry)
        {
            UnitRepository.Add(entry);
            UnitRepository.Commit();
            return entry;
        }

        public async Task<IEnumerable<Unit>> GetAll()
        {
            return UnitRepository.GetAll();
        }

        public async Task<Unit> GetById(int id)
        {
            return UnitRepository.GetById(id);
        }

        public async Task<bool> Remove(int id)
        {
            UnitRepository.Remove(id);
            UnitRepository.Commit();
            return true;
        }

        public async Task<Unit> Update(Unit entry)
        {
            var entryToUpdate = await GetById(entry.Id);
            entry.Updated = DateTime.Now;
            entryToUpdate.Name = entry.Name;
            entryToUpdate.Description = entry.Description;

            return UnitRepository.Update(entryToUpdate);
        }
    }
}
