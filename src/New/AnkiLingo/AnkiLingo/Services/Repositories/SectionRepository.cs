using AnkiLingo.Data;

namespace AnkiLingo.Services.Repositories
{
    public interface ISectionRepository
    {
        IEnumerable<Section> GetAllSections();
        Section GetSectionById(Guid id);
        void AddSection(Section section);
        void UpdateSection(Section section);
        void DeleteSection(Guid id);
    }

    public class SectionRepository : ISectionRepository
    {
        private readonly List<Section> _sections = new List<Section>();

        public IEnumerable<Section> GetAllSections()
        {
            return _sections;
        }

        public Section GetSectionById(Guid id)
        {
            return _sections.FirstOrDefault(s => s.Id == id);
        }

        public void AddSection(Section section)
        {
            _sections.Add(section);
        }

        public void UpdateSection(Section section)
        {
            var existingSection = GetSectionById(section.Id);
            if (existingSection != null)
            {
                _sections.Remove(existingSection);
                _sections.Add(section);
            }
        }

        public void DeleteSection(Guid id)
        {
            var section = GetSectionById(id);
            if (section != null)
            {
                _sections.Remove(section);
            }
        }
    }
}
