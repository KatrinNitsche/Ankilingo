using AnkiLingo.Data;

namespace AnkiLingo.Services.Repositories
{
    public interface IUnitRepository
    {
        IEnumerable<Unit> GetAllUnits();
        Unit GetUnitById(Guid id);
        void AddUnit(Unit unit);
        void UpdateUnit(Unit unit);
        void DeleteUnit(Guid id);
    }

    public class UnitRepository : IUnitRepository
    {
        private readonly List<Unit> _units = new List<Unit>();

        public IEnumerable<Unit> GetAllUnits()
        {
            return _units;
        }

        public Unit GetUnitById(Guid id)
        {
            return _units.FirstOrDefault(u => u.Id == id);
        }

        public void AddUnit(Unit unit)
        {
            _units.Add(unit);
        }

        public void UpdateUnit(Unit unit)
        {
            var existingUnit = GetUnitById(unit.Id);
            if (existingUnit != null)
            {
                _units.Remove(existingUnit);
                _units.Add(unit);
            }
        }

        public void DeleteUnit(Guid id)
        {
            var unit = GetUnitById(id);
            if (unit != null)
            {
                _units.Remove(unit);
            }
        }
    }
}
