using AnkiLingo.Data;
using AnkiLingo.Services.Repositories;
using AnkiLingoExcelService.Data;

namespace AnkiLingoBackendService
{
    public interface IDatabaseService
    {
        Task<UserData> GetUserData(Guid userId);
        Task<IEnumerable<string>> GetCourseNames();
        Task<CourseData> GetCourseContent(Guid userId, string courseName);
        Task<CourseData> GetCourseDetails(Guid userId, string courseName);
        Task<bool> AddCourse(Course course);
        Task<bool> UpdateEntry(Guid userId, EntryData entry);
        Task<bool> UpdateUserData(Guid userId, int? XP = null, TimeOnly? duration = null);
        Task<bool> UpdateUserData(Guid userId, string currenCourseName);
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
                // check if course already exists
                var existingCourse = await courseRepository.GetCourseByName(course.Name);
                if (existingCourse != null)
                {
                    await courseRepository.UpdateCourse(course);
                }
                else
                {
                    await courseRepository.AddCourse(course);
                }
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
            if (string.IsNullOrEmpty(courseName)) return new CourseData();

            var course = await courseRepository.GetCourseByName(courseName);
            var userCourseData = await userCourseDataRepository.GetUserCourseDataAsync(userId, course.Id);

            var courseData = new CourseData
            {
                Name = course.Name,
                Description = course.Description,
                Icon = course.Icon,
                Sections = course.Sections.Select(s => new SectionData
                {
                    Name = s.Name,
                    Description = s.Description,
                    Units = s.Units.Select(u => new UnitData
                    {
                        Name = u.Name,
                        Description = u.Description,
                        Entries = u.Entries.Select(e => new EntryData
                        {
                            id = e.Id,
                            CourseId = course.Id,
                            SectionId = s.Id,
                            UnitId = u.Id,
                            Value1 = e.Value1,
                            Value2 = e.Value2
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            if (course.Images != null)
            {
                course.Images = course.Images.ToList();
            }
            else
            {
                course.Images = new List<ImageData>();
            }

            // add missing entries to userCourseData
            foreach (SectionData section in courseData.Sections)
            {
                foreach (UnitData unit in section.Units)
                {
                    foreach (EntryData entry in unit.Entries)
                    {
                        var existingEntry = userCourseData.FirstOrDefault(e => e.EntryId == entry.id);
                        if (existingEntry != null)
                        {
                            entry.LastReviewed = existingEntry.LastReviewed;
                            entry.ReviewCount = existingEntry.ReviewCount;
                            entry.LevelOfKnowledge = existingEntry.LevelOfKnowledge;
                        }
                        else
                        {
                            entry.LastReviewed = DateTime.MinValue;
                            entry.ReviewCount = 0;
                            entry.LevelOfKnowledge = 0;
                        }
                    }
                }
            }

            return courseData;
        }

        public async Task<CourseData> GetCourseDetails(Guid userId, string courseName)
        {
            var course = await courseRepository.GetCourseByName(courseName);

            var courseData = new CourseData
            {
                Name = course.Name,
                Description = course.Description,
                Icon = course.Icon,
                Sections = course.Sections.Select(s => new SectionData
                {
                    Name = s.Name,
                    Description = s.Description,
                    Units = s.Units.Select(u => new UnitData
                    {
                        Name = u.Name,
                        Description = u.Description
                    }).ToList()
                }).ToList()
            };

            return courseData;
        }

        public async Task<bool> UpdateEntry(Guid userId, EntryData entry)
        {
            try
            {
                var existingEntry = await userCourseDataRepository.GetUserCourseDataEntry(userId, entry.CourseId, entry.id);

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
                }
                else
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

        public async Task<bool> UpdateUserData(Guid userId, string currenCourseName)
        {
            try
            {
                // get existing user data
                var existingData = await userDataRepository.GetUserDataAsync(userId);
                existingData.CurrentCourse = currenCourseName;
                await userDataRepository.UpdateUserDataAsync(existingData);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user data: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateUserData(Guid userId, int? XP = null, TimeOnly? duration = null)
        {
            try
            {
                // get existing user data
                var existingData = await userDataRepository.GetUserDataAsync(userId);

                if (XP.HasValue)
                {
                    existingData.XPCount += XP.Value;
                    existingData.GemsCount += XP.Value / 10; // Example: 1 gem for every 100 XP
                }

                // Increase the current streak by 1 if the last study was longer than 24 hours ago
                // but now longer than 48 hours ago
                if (existingData.LastStudy < DateTime.Now.AddHours(-24) &&
                    existingData.LastStudy >= DateTime.Now.AddHours(-48))
                {
                    existingData.StreakLength += 1;
                }
                else if (existingData.LastStudy < DateTime.Now.AddHours(-48))
                {
                    existingData.StreakLength = 1; // reset streak
                }

                existingData.LastStudy = DateTime.Now;

                await userDataRepository.UpdateUserDataAsync(existingData);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user data: {UserId}", userId);
                return false;
            }
        }

    }
}
