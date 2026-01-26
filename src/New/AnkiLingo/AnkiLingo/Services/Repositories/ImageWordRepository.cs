using AnkiLingo.Data;
using AnkiLingoExcelService.Data;

namespace AnkiLingo.Services.Repositories
{
    public interface IImageWordRepository
    {
        IEnumerable<ImageWord> GetAllSections();
        ImageWord GetImageWordById(Guid id);
        Task AddImageWord(ImageWord section);
        Task UpdateImageWord(ImageWord section);
        Task DeleteImageWord(Guid id);
        Task<ImageWord> GetImageWordByValue(EntryData value);
    }

    public class ImageWordRepository : IImageWordRepository
    {
        private readonly ApplicationDbContext _context;
        public ImageWordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ImageWord> GetAllSections()
        {
            return _context.ImageWords.ToList();
        }
        public ImageWord GetImageWordById(Guid id)
        {
            return _context.ImageWords.Find(id);
        }
        public async Task AddImageWord(ImageWord section)
        {
            _context.ImageWords.Add(section);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateImageWord(ImageWord section)
        {
            _context.ImageWords.Update(section);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageWord(Guid id)
        {
            var section = _context.ImageWords.Find(id);
            if (section != null)
            {
                _context.ImageWords.Remove(section);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<ImageWord> GetImageWordByValue(EntryData value)
        {
            return _context.ImageWords
                .FirstOrDefault(iw => iw.Id == value.id);
        }
    }
}
