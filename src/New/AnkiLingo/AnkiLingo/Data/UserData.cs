using System.ComponentModel.DataAnnotations;

namespace AnkiLingoExcelService.Data
{
    public class UserData
    {
        [Key]
        public Guid UserId { get; set; }
        public int StreakLength { get; set; }
        public int GemsCount { get; set; }
        public string? CurrentCourse { get; set; } 
        public int XPCount { get; set; }
        public DateTime LastStudy { get; set; } = DateTime.MinValue;
    }

    public class UserCourseData
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public Guid SectionId { get; set; }
        public Guid UnitId { get; set; }
        public Guid EntryId { get; set; }
        public int LevelOfKnowledge { get; set; }
        public DateTime LastReviewed { get; set; }
        public int ReviewCount { get; set; } = 0;
    }
}
