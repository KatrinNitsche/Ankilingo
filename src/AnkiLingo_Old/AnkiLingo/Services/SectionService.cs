using AnkiLingo.Data;
using AnkiLingo.Repositories;

namespace AnkiLingo.Services
{
    public class SectionService : ISectionService
    {
        private SectionRepository SectionRepository;

        public SectionService(DataContext context)
        {
            SectionRepository = new SectionRepository(context);
        }

        public async Task<Section> Add(Section entry)
        {
            SectionRepository.Add(entry);
            SectionRepository.Commit();
            return entry;
        }

        public async Task<IEnumerable<Section>> GetAll()
        {
            return SectionRepository.GetAll();
        }

        public async Task<Section> GetById(int id)
        {
            return SectionRepository.GetById(id);
        }

        public async Task<bool> Remove(int id)
        {
            SectionRepository.Remove(id);
            SectionRepository.Commit();
            return true;
        }

        public async Task<Section> Update(Section entry)
        {
            var entryToUpdate = await GetById(entry.Id);
            entry.Updated = DateTime.Now;
            entryToUpdate.Name = entry.Name;
            entryToUpdate.Description = entry.Description;

            return SectionRepository.Update(entryToUpdate);
        }
    }
}
