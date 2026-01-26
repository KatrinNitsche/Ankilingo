using AnkiLingo.Data;
using AnkiLingoExcelService.Data;
using Microsoft.EntityFrameworkCore;

namespace AnkiLingo.Services.Repositories
{
    /// <summary>
    /// Interface for course repository (CRUD operations for courses).
    /// </summary>
    public interface ICourseRepository
    {
        IEnumerable<Course> GetAllCourses();
        Task<IEnumerable<CourseDetails>> GetCourseDetailsAsync();
        Task<Course> GetCourseById(Guid id);
        Task<Course> GetCourseByName(string courseName);
        Task<bool> AddCourse(Course course);
        Task<bool> UpdateCourse(Course course);
        Task<bool> DeleteCourse(Guid id);
    }

    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CourseRepository> _logger;

        public CourseRepository(ApplicationDbContext dbContext, ILogger<CourseRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IEnumerable<Course> GetAllCourses()
        {
            return _dbContext.Courses.ToList();
        }

        public async Task<IEnumerable<CourseDetails>> GetCourseDetailsAsync()
        {
            var data = await _dbContext.Courses.Select(c => new CourseDetails
            {
                id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Icon = c.Icon
            }).ToListAsync();
            return data;
        }

        public async Task<Course> GetCourseById(Guid id)
        {
            return await _dbContext.Courses.FindAsync(id);
        }

        public async Task<Course> GetCourseByName(string courseName)
        {
            if (string.IsNullOrWhiteSpace(courseName)) return null;

            return await _dbContext.Courses
                .Include(c => c.Images)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Units)
                        .ThenInclude(u => u.Entries)
                .FirstOrDefaultAsync(c => c.Name != null && c.Name == courseName);
        }

        public async Task<bool> AddCourse(Course course)
        {
            try
            {
                _logger.LogInformation("AddCourse starting for {CourseName}", course?.Name);

                if (course.Id == Guid.Empty) course.Id = Guid.NewGuid();

                if (course.Sections != null)
                {
                    foreach (Section section in course.Sections)
                    {
                        if (section.Id == Guid.Empty) section.Id = Guid.NewGuid();
                        section.CourseId = course.Id;
                        section.Name = section.Name ?? string.Empty;
                        section.Created = DateTime.UtcNow;
                        section.Updated = DateTime.UtcNow;     
                        section.Order = section.Order;
                        if (section.Units != null)
                        {
                            foreach (Unit unit in section.Units)
                            {
                                if (unit.Id == Guid.Empty) unit.Id = Guid.NewGuid();
                                unit.Name = unit.Name ?? string.Empty;
                                unit.Created = DateTime.UtcNow;
                                unit.Updated = DateTime.UtcNow;
                                unit.SectionId = section.Id;    
                                unit.Order = unit.Order;
                                if (unit.Entries != null)
                                {
                                    var index = 0;
                                    foreach (Entry entry in unit.Entries)
                                    {
                                        if (entry.Id == Guid.Empty) entry.Id = Guid.NewGuid();
                                        entry.Created = DateTime.UtcNow;
                                        entry.Updated = DateTime.UtcNow;
                                        entry.Description = entry.Description ?? string.Empty;
                                        entry.Name = entry.Name ?? string.Empty;
                                        entry.Unit = unit;
                                        entry.Order = index;
                                        
                                        index++;
                                    }
                                }
                            }
                        }
                    }
                }

                if (course.Images != null)
                {
                    foreach (var img in course.Images)
                    {
                        if (img.Id == Guid.Empty) img.Id = Guid.NewGuid();
                        img.CourseId = course.Id;

                        // Ensure every ImageWord (cover) is fully associated with the course and parent image.
                        if (img.ImageCovers != null)
                        {
                            foreach (var cover in img.ImageCovers)
                            {
                                if (cover.Id == Guid.Empty) cover.Id = Guid.NewGuid();

                                // set CourseId and navigation so the FK is valid
                                cover.CourseId = course.Id;
                                cover.Course = course;

                                // if there's a navigation/property to the parent image, set it as well
                                // (works whether EF uses a FK property or navigation)
                                try
                                {
                                    // assign parent image navigation if property exists
                                    cover.GetType().GetProperty("ImageData")?.SetValue(cover, img);
                                    // assign ImageDataId property if it exists
                                    var prop = cover.GetType().GetProperty("ImageDataId");
                                    if (prop != null) prop.SetValue(cover, img.Id);
                                }
                                catch
                                {
                                    // ignore reflection failures; primary assignment is cover.Course and cover.CourseId
                                }

                                // If the cover has a Value (EntryData), make sure it references the new course and has an id
                                if (cover.Value != null)
                                {
                                    if (cover.Value.id == Guid.Empty) cover.Value.id = Guid.NewGuid();
                                    cover.Value.CourseId = course.Id;
                                    // it's useful to set EntryId if that is used as an int index — but keep existing EntryId
                                }
                            }
                        }
                    }
                }

                await _dbContext.Courses.AddAsync(course);

                // dump ChangeTracker before save
                var entries = _dbContext.ChangeTracker.Entries().ToList();
                _logger.LogDebug("ChangeTracker entries before SaveChanges: {Count}", entries.Count);
                foreach (var e in entries)
                {
                    _logger.LogDebug("Entity: {Type}, State: {State}", e.Entity.GetType().Name, e.State);
                }

                var rows = await _dbContext.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync returned {Rows} rows; course id {CourseId}", rows, course.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddCourse failed for {CourseName}", course?.Name);
                return false;
            }
        }

        public async Task<bool> UpdateCourse(Course course)
        {
            try
            {
                // Load existing course and related children
                var existingCourse = await _dbContext.Courses
                    .Include(c => c.Images)
                        .ThenInclude(i => i.ImageCovers)
                    .Include(c => c.Sections)
                        .ThenInclude(s => s.Units)
                            .ThenInclude(u => u.Entries)
                    .FirstOrDefaultAsync(c => c.Id == course.Id || (c.Name != null && c.Name == course.Name));

                if (existingCourse == null)
                {
                    _logger.LogWarning("UpdateCourse: course not found: {CourseName}/{CourseId}", course?.Name, course?.Id);
                    return false;
                }

                // Update root scalar properties
                _dbContext.Entry(existingCourse).CurrentValues.SetValues(course);
                existingCourse.Updated = DateTime.UtcNow;

                // --- Sections synchronization ---
                var incomingSections = course.Sections ?? new List<Section>();
                var existingSections = existingCourse.Sections ?? new List<Section>();

                // Remove sections no longer present
                var incomingSectionIds = incomingSections.Select(s => s.Id).Where(id => id != Guid.Empty).ToHashSet();
                foreach (var toRemove in existingSections.Where(s => !incomingSectionIds.Contains(s.Id)).ToList())
                {
                    _dbContext.Sections.Remove(toRemove);
                }

                // Add or update incoming sections
                foreach (var incomingSection in incomingSections)
                {
                    // new section
                    if (incomingSection.Id == Guid.Empty || !existingSections.Any(s => s.Id == incomingSection.Id))
                    {
                        if (incomingSection.Id == Guid.Empty) incomingSection.Id = Guid.NewGuid();
                        incomingSection.CourseId = existingCourse.Id;
                        incomingSection.Created = DateTime.UtcNow;
                        incomingSection.Updated = DateTime.UtcNow;

                        // fix units & entries for new section
                        if (incomingSection.Units != null)
                        {
                            foreach (var unit in incomingSection.Units)
                            {
                                if (unit.Id == Guid.Empty) unit.Id = Guid.NewGuid();
                                unit.SectionId = incomingSection.Id;
                                unit.Created = DateTime.UtcNow;
                                unit.Updated = DateTime.UtcNow;

                                if (unit.Entries != null)
                                {
                                    foreach (var entry in unit.Entries)
                                    {
                                        if (entry.Id == Guid.Empty) entry.Id = Guid.NewGuid();                                       
                                        entry.Created = DateTime.UtcNow;
                                        entry.Updated = DateTime.UtcNow;
                                    }
                                }
                            }
                        }

                        existingCourse.Sections.Add(incomingSection);
                    }
                    else
                    {
                        // update existing section
                        var existingSection = existingSections.First(s => s.Id == incomingSection.Id);
                        _dbContext.Entry(existingSection).CurrentValues.SetValues(incomingSection);
                        existingSection.Updated = DateTime.UtcNow;

                        // --- Units synchronization within section ---
                        var incomingUnits = incomingSection.Units ?? new List<Unit>();
                        var existingUnits = existingSection.Units ?? new List<Unit>();

                        var incomingUnitIds = incomingUnits.Select(u => u.Id).Where(id => id != Guid.Empty).ToHashSet();
                        foreach (var eu in existingUnits.Where(u => !incomingUnitIds.Contains(u.Id)).ToList())
                        {
                            _dbContext.Units.Remove(eu);
                        }

                        foreach (var incomingUnit in incomingUnits)
                        {
                            if (incomingUnit.Id == Guid.Empty || !existingUnits.Any(u => u.Id == incomingUnit.Id))
                            {
                                if (incomingUnit.Id == Guid.Empty) incomingUnit.Id = Guid.NewGuid();
                                incomingUnit.SectionId = existingSection.Id;
                                incomingUnit.Created = DateTime.UtcNow;
                                incomingUnit.Updated = DateTime.UtcNow;

                                if (incomingUnit.Entries != null)
                                {
                                    foreach (var entry in incomingUnit.Entries)
                                    {
                                        if (entry.Id == Guid.Empty) entry.Id = Guid.NewGuid();                                     
                                        entry.Created = DateTime.UtcNow;
                                        entry.Updated = DateTime.UtcNow;
                                    }
                                }

                                existingSection.Units.Add(incomingUnit);
                            }
                            else
                            {
                                var existingUnit = existingUnits.First(u => u.Id == incomingUnit.Id);
                                _dbContext.Entry(existingUnit).CurrentValues.SetValues(incomingUnit);
                                existingUnit.Updated = DateTime.UtcNow;

                                // --- Entries synchronization within unit ---
                                var incomingEntries = incomingUnit.Entries ?? new List<Entry>();
                                var existingEntries = existingUnit.Entries ?? new List<Entry>();

                                var incomingEntryIds = incomingEntries.Select(e => e.Id).Where(id => id != Guid.Empty).ToHashSet();
                                foreach (var ee in existingEntries.Where(e => !incomingEntryIds.Contains(e.Id)).ToList())
                                {
                                    _dbContext.Entries.Remove(ee);
                                }

                                foreach (var incomingEntry in incomingEntries)
                                {
                                    if (incomingEntry.Id == Guid.Empty || !existingEntries.Any(e => e.Id == incomingEntry.Id))
                                    {
                                        if (incomingEntry.Id == Guid.Empty) incomingEntry.Id = Guid.NewGuid();
                                        incomingEntry.Created = DateTime.UtcNow;
                                        incomingEntry.Updated = DateTime.UtcNow;
                                        existingUnit.Entries.Add(incomingEntry);
                                    }
                                    else
                                    {
                                        var existingEntry = existingEntries.First(e => e.Id == incomingEntry.Id);
                                        _dbContext.Entry(existingEntry).CurrentValues.SetValues(incomingEntry);
                                        existingEntry.Updated = DateTime.UtcNow;
                                    }
                                }
                            }
                        }
                    }
                }

                // --- Images synchronization ---
                var incomingImages = course.Images ?? new List<ImageData>();
                var existingImages = existingCourse.Images ?? new List<ImageData>();

                var incomingImageIds = incomingImages.Select(i => i.Id).Where(id => id != Guid.Empty).ToHashSet();
                foreach (var toRemove in existingImages.Where(i => !incomingImageIds.Contains(i.Id)).ToList())
                {
                    _dbContext.Images.Remove(toRemove);
                }

                foreach (var incomingImage in incomingImages)
                {
                    if (incomingImage.Id == Guid.Empty || !existingImages.Any(i => i.Id == incomingImage.Id))
                    {
                        if (incomingImage.Id == Guid.Empty) incomingImage.Id = Guid.NewGuid();
                        incomingImage.CourseId = existingCourse.Id;

                        if (incomingImage.ImageCovers != null)
                        {
                            foreach (var cover in incomingImage.ImageCovers)
                            {
                                if (cover.Id == Guid.Empty) cover.Id = Guid.NewGuid();
                                cover.CourseId = existingCourse.Id;
                                // assign back-reference if needed
                            }
                        }

                        existingCourse.Images.Add(incomingImage);
                    }
                    else
                    {
                        var existingImage = existingImages.First(i => i.Id == incomingImage.Id);
                        _dbContext.Entry(existingImage).CurrentValues.SetValues(incomingImage);
                        existingImage.CourseId = existingCourse.Id;

                        // sync image covers
                        var incomingCovers = incomingImage.ImageCovers ?? new List<ImageWord>();
                        var existingCovers = existingImage.ImageCovers ?? new List<ImageWord>();

                        var incomingCoverIds = incomingCovers.Select(c => c.Id).Where(id => id != Guid.Empty).ToHashSet();
                        foreach (var ec in existingCovers.Where(c => !incomingCoverIds.Contains(c.Id)).ToList())
                        {
                            _dbContext.ImageWords.Remove(ec);
                        }

                        foreach (var incomingCover in incomingCovers)
                        {
                            if (incomingCover.Id == Guid.Empty || !existingCovers.Any(c => c.Id == incomingCover.Id))
                            {
                                if (incomingCover.Id == Guid.Empty) incomingCover.Id = Guid.NewGuid();
                                incomingCover.CourseId = existingCourse.Id;
                                existingImage.ImageCovers.Add(incomingCover);
                            }
                            else
                            {
                                var existingCover = existingCovers.First(c => c.Id == incomingCover.Id);
                                _dbContext.Entry(existingCover).CurrentValues.SetValues(incomingCover);
                                existingCover.CourseId = existingCourse.Id;
                            }
                        }
                    }
                }

                // Save all changes
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("UpdateCourse successful for {CourseId}", existingCourse.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateCourse failed for {CourseName} ({CourseId})", course?.Name, course?.Id);
                return false;
            }
        }

        public async Task<bool> DeleteCourse(Guid id)
        {
            var course = await GetCourseById(id);
            if (course != null)
            {
                _dbContext.Courses.Remove(course);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
