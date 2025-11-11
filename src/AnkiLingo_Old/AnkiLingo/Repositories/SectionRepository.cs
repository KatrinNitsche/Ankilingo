using AnkiLingo.Data;

namespace AnkiLingo.Repositories
{
    public class SectionRepository : BaseRepository<Section>
    {
        public SectionRepository(DataContext context) : base(context) { }
        public override IEnumerable<Section> GetAll() => Context.Sections;
        public override Section? GetById(int id) => Context.Sections.FirstOrDefault(x => x.Id == id);
    }
}
