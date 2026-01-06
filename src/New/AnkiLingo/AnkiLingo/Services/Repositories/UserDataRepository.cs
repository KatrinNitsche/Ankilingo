using AnkiLingo.Data;
using AnkiLingoExcelService.Data;

namespace AnkiLingo.Services.Repositories
{
    public interface IUserDataRepository
    {
        Task<UserData> GetUserDataAsync(Guid userId);
        Task AddUserDataAsync(UserData userData);
        Task UpdateUserDataAsync(UserData userData);
        Task DeleteUserDataAsync(Guid userId);
    }

    public class UserDataRepository : IUserDataRepository
    {
        private readonly ApplicationDbContext _context;
        public UserDataRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<UserData> GetUserDataAsync(Guid userId)
        {
            var data = await _context.UserData.FindAsync(userId);

            if (data == null)
            {
                data = new UserData
                {
                    UserId = userId,
                    StreakLength = 0,
                    GemsCount = 0,
                    CurrentCourse = string.Empty,
                    XPCount = 0,
                    LastStudy = DateTime.MinValue
                };
                _context.UserData.Add(data);
                await _context.SaveChangesAsync();
            }

            return data;
        }
        public async Task AddUserDataAsync(UserData userData)
        {
            _context.UserData.Add(userData);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateUserDataAsync(UserData userData)
        {
            _context.UserData.Update(userData);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUserDataAsync(Guid userId)
        {
            var userData = await _context.UserData.FindAsync(userId);
            if (userData != null)
            {
                _context.UserData.Remove(userData);
                await _context.SaveChangesAsync();
            }
        }
    }
}
