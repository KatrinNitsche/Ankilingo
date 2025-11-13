using AnkiLingo.Services;
using AnkiLingo.Services.Repositories;
using AnkiLingoExcelService.Data;

namespace AnkiLingoBackendService
{
    public interface IDatabaseService
    {
        Task<UserData> GetUserData(Guid userId);
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly IUserDataRepository userDataRepository;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IUserDataRepository userDataRepository, ILogger<DatabaseService> logger)
        {
            this.userDataRepository = userDataRepository;
            _logger = logger;
        }

        public async Task<UserData> GetUserData(Guid userId)
        {
           return await userDataRepository.GetUserDataAsync(userId);
        }
    }
}
