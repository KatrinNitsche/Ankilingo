using AnkiLingo.Data;

namespace AnkiLingo.Repositories
{
    public class EntryRepository : BaseRepository<Entry>
    {
        public EntryRepository(DataContext context) : base(context) { }
        public override IEnumerable<Entry> GetAll() => Context.Entries;
        public override Entry? GetById(int id) => Context.Entries.FirstOrDefault(x => x.Id == id);
    }
}
