using AnkiLingo.Data;
using AnkiLingo.Repositories;

namespace AnkiLingo.Services
{
    public class EntryService : IEntryService
    {
        private EntryRepository EntryRepository;

        public EntryService(DataContext context)
        {
            EntryRepository = new EntryRepository(context);
        }

        public async Task<Entry> Add(Entry entry)
        {
            EntryRepository.Add(entry);
            EntryRepository.Commit();
            return entry;
        }

        public async Task<IEnumerable<Entry>> GetAll()
        {
            return EntryRepository.GetAll();
        }

        public async Task<Entry> GetById(int id)
        {
            return EntryRepository.GetById(id);
        }

        public async Task<bool> Remove(int id)
        {
            EntryRepository.Remove(id);
            EntryRepository.Commit();
            return true;
        }

        public async Task<Entry> Update(Entry entry)
        {
            var entryToUpdate = await GetById(entry.Id);
            entry.Updated = DateTime.Now;
            entryToUpdate.Name = entry.Name;
            entryToUpdate.Description = entry.Description;

            return EntryRepository.Update(entryToUpdate);
        }
    }
}
