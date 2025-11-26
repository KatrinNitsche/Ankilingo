using System.ComponentModel.DataAnnotations;

namespace AnkiLingoExcelService.Data
{
    public class UserData
    {
        [Key]
        public Guid UserId { get; set; }
        public int StreakLength { get; set; }
        public int GemsCount { get; set; }
        public string CurrentCourse { get; set; }
        public int XPCount { get; set; }
        public DateTime LastStudy { get; set; } = DateTime.MinValue;
    }

    public class UserCourseData
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Guid UserId { get; set; }
        public int SectionId { get; set; }
        public int UnitId { get; set; }
        public int EntryId { get; set; }
        public int LevelOfKnowledge { get; set; }
        public DateTime LastReviewed { get; set; }
        public int ReviewCount { get; set; } = 0;
    }
}
