using AnkiLingo.Data;
using AnkiLingo.Services;
using AnkiLingo.Services.Repositories;
using AnkiLingoExcelService.Data;

namespace AnkiLingoBackendService
{
    public interface IDatabaseService
    {
        Task<UserData> GetUserData(Guid userId);
        Task<IEnumerable<string>> GetCourseNames();
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly IUserDataRepository userDataRepository;
        private readonly ICourseRepository courseRepository;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IUserDataRepository userDataRepository, ICourseRepository courseRepository, ILogger<DatabaseService> logger)
        {
            this.userDataRepository = userDataRepository;
            this.courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task<UserData> GetUserData(Guid userId)
        {
           return await userDataRepository.GetUserDataAsync(userId);
        }

        public async Task<IEnumerable<string>> GetCourseNames()
        {
            return await courseRepository.GetCourseNamesAsync();
        }
    }
}
