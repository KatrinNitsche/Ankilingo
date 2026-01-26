using AnkiLingo.Data;

namespace AnkiLingo.Services.Repositories
{
    public interface IImageRepository
    {
        IEnumerable<ImageData> GetAllSections();
        ImageData GetImageDataById(Guid id);
        Task AddImageData(ImageData section);
        Task UpdateImageData(ImageData section);
        Task DeleteImageData(Guid id);
        Task<ImageData> GetImageByNameAndCourseId(string imageName, Guid id);
    }

    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;
        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ImageData> GetAllSections()
        {
            return _context.Images.ToList();
        }

        public ImageData GetImageDataById(Guid id)
        {
            return _context.Images.FirstOrDefault(image => image.Id == id);
        }

        public async Task AddImageData(ImageData section)
        {
            _context.Images.Add(section);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateImageData(ImageData section)
        {
            _context.Images.Update(section);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageData(Guid id)
        {
            var image = _context.Images.FirstOrDefault(i => i.Id == id);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ImageData> GetImageByNameAndCourseId(string imageName, Guid id)
        {
            return _context.Images.FirstOrDefault(image => image.ImageName == imageName && image.CourseId == id);
        }
    }
}
