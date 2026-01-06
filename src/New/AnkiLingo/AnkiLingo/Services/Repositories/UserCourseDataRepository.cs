using AnkiLingo.Data;
using AnkiLingoExcelService.Data;
using Microsoft.EntityFrameworkCore;

namespace AnkiLingo.Services.Repositories
{
    public interface IUserCourseDataRepository
    {
        Task<IEnumerable<UserCourseData>> GetUserCourseDataAsync(Guid userId, Guid courseId);
        Task AddUserCourseDataAsync(UserCourseData userCourseData);
        Task UpdateUserCourseDataAsync(UserCourseData userCourseData);
        Task DeleteUserCourseDataAsync(Guid userId, Guid courseId);
        Task<UserCourseData> GetUserCourseDataEntry(Guid userId, Guid courseId, Guid entryId);
    }

    public class UserCourseDataRepository : IUserCourseDataRepository
    {
        private readonly ApplicationDbContext _context;

        public UserCourseDataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserCourseData>> GetUserCourseDataAsync(Guid userId, Guid courseId)
        {
            return await _context.UserCourseData
                .Where(ucd => ucd.UserId == userId && ucd.CourseId == courseId)
                .ToListAsync();
        }
        public async Task AddUserCourseDataAsync(UserCourseData userCourseData)
        {
            _context.UserCourseData.Add(userCourseData);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateUserCourseDataAsync(UserCourseData userCourseData)
        {
            _context.UserCourseData.Update(userCourseData);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUserCourseDataAsync(Guid userId, Guid courseId)
        {
            var userCourseData = await _context.UserCourseData
                .FirstOrDefaultAsync(ucd => ucd.UserId == userId && ucd.CourseId == courseId);
            if (userCourseData != null)
            {
                _context.UserCourseData.Remove(userCourseData);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<UserCourseData?> GetUserCourseDataEntry(Guid userId, Guid courseId, Guid entryId)
        {
            return await _context.UserCourseData
                .FirstOrDefaultAsync(ucd => ucd.UserId == userId && ucd.CourseId == courseId && ucd.EntryId == entryId);
        }
    }
}
