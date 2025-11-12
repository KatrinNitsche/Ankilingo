using AnkiLingo.Data;

namespace AnkiLingo.Services.Repositories
{
    public interface IEntryRepository
    {
        IEnumerable<Entry> GetAllEntries();
        Entry GetEntryById(int id);
        void AddEntry(Entry entry);
        void UpdateEntry(Entry entry);
        void DeleteEntry(int id);
    }

    public class EntryRepository : IEntryRepository
    {
        private readonly List<Entry> _entries = new List<Entry>();
        public IEnumerable<Entry> GetAllEntries()
        {
            return _entries;
        }
        public Entry GetEntryById(int id)
        {
            return _entries.FirstOrDefault(e => e.Id == id);
        }
        public void AddEntry(Entry entry)
        {
            _entries.Add(entry);
        }
        public void UpdateEntry(Entry entry)
        {
            var existingEntry = GetEntryById(entry.Id);
            if (existingEntry != null)
            {
                _entries.Remove(existingEntry);
                _entries.Add(entry);
            }
        }
        public void DeleteEntry(int id)
        {
            var entry = GetEntryById(id);
            if (entry != null)
            {
                _entries.Remove(entry);
            }
        }
    }
}
