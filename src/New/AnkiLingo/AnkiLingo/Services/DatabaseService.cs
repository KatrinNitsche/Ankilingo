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
        Task<CourseData> GetCourseContent(Guid userId, string courseName);
        Task<bool> AddCourse(Course course);
        Task<bool> UpdateEntry(Guid userId, EntryData entry);
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly IUserDataRepository userDataRepository;
        private readonly ICourseRepository courseRepository;
        private readonly IUserCourseDataRepository userCourseDataRepository;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IUserDataRepository userDataRepository,
            ICourseRepository courseRepository,
            ILogger<DatabaseService> logger,
            IUserCourseDataRepository userCourseDataRepository)
        {
            _logger = logger;
            this.userDataRepository = userDataRepository;
            this.courseRepository = courseRepository;
            this.userCourseDataRepository = userCourseDataRepository;
        }

        public async Task<bool> AddCourse(Course course)
        {
            try
            {
                await courseRepository.AddCourse(course);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding course: {CourseName}", course.Name);
                return false;
            }
        }

        public async Task<UserData> GetUserData(Guid userId)
        {
            return await userDataRepository.GetUserDataAsync(userId);
        }

        public async Task<IEnumerable<string>> GetCourseNames()
        {
            return await courseRepository.GetCourseNamesAsync();
        }

        public async Task<CourseData> GetCourseContent(Guid userId, string courseName)
        {
            var course = await courseRepository.GetCourseByName(courseName);
            var userCourseData = await userCourseDataRepository.GetUserCourseDataAsync(userId, course.Id);

            var courseData = new CourseData
            {
                Name = course.Name,
                Sections = course.Sections.Select(s => new SectionData
                {
                    Name = s.Name,
                    Units = s.Units.Select(u => new UnitData
                    {
                        Name = u.Name,
                        Entries = u.Entries.Select(e => new EntryData
                        {                           
                            Value1 = e.Value1,
                            Value2 = e.Value2,
                            CourseId = course.Id,
                            SectionId = s.Id,
                            UnitId = u.Id,
                            LastReviewed = userCourseData.FirstOrDefault(ucd => ucd.EntryId == e.Id)?.LastReviewed ?? DateTime.MinValue,
                            ReviewCount = userCourseData.FirstOrDefault(ucd => ucd.EntryId == e.Id)?.ReviewCount ?? 0,
                            LevelOfKnowledge = userCourseData.FirstOrDefault(ucd => ucd.EntryId == e.Id)?.LevelOfKnowledge ?? 0
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return courseData;
        }

        public async Task<bool> UpdateEntry(Guid userId, EntryData entry)
        {
            try
            {
                var userCourseData = await userCourseDataRepository.GetUserCourseDataAsync(userId, entry.CourseId);
                var existingEntry = userCourseData.FirstOrDefault(ucd => ucd.CourseId == entry.CourseId &&
                                                                  ucd.SectionId == entry.SectionId &&
                                                                  ucd.UnitId == entry.UnitId &&
                                                                  ucd.EntryId == entry.id);
                                
                if (existingEntry == null)
                {
                    var newEntry = new UserCourseData
                    {
                        UserId = userId,
                        CourseId = entry.CourseId,
                        SectionId = entry.SectionId,
                        UnitId = entry.UnitId,
                        EntryId = entry.id,
                        LastReviewed = entry.LastReviewed,
                        ReviewCount = entry.ReviewCount,
                        LevelOfKnowledge = entry.LevelOfKnowledge
                    };

                    await userCourseDataRepository.AddUserCourseDataAsync(newEntry);
                    existingEntry = userCourseData.FirstOrDefault(ucd => ucd.CourseId == entry.CourseId &&
                                                              ucd.SectionId == entry.SectionId &&
                                                              ucd.UnitId == entry.UnitId &&
                                                              ucd.EntryId == entry.id);
                }

                if (existingEntry != null)
                {
                    existingEntry.LastReviewed = entry.LastReviewed;
                    existingEntry.ReviewCount = entry.ReviewCount;
                    existingEntry.LevelOfKnowledge = entry.LevelOfKnowledge;

                    await userCourseDataRepository.UpdateUserCourseDataAsync(existingEntry);
                }
             
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entry: {EntryId}", entry.id);
                return false;
            }
        }
    }
}
