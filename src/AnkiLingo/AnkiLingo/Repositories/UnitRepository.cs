using AnkiLingo.Data;

namespace AnkiLingo.Repositories
{
    public class UnitRepository : BaseRepository<Unit>
    {
        public UnitRepository(DataContext context) : base(context) { }
        public override IEnumerable<Unit> GetAll() => Context.Units;
        public override Unit? GetById(int id) => Context.Units.FirstOrDefault(x => x.Id == id);
    }
}
