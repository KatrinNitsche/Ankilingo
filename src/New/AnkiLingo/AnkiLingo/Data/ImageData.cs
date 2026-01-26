using AnkiLingoExcelService.Data;
using System.ComponentModel.DataAnnotations;

namespace AnkiLingo.Data
{
    public class ImageData
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public List<ImageWord> ImageCovers { get; set; } = new List<ImageWord>();
    }

    public class ImageWord
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
        public int EntryId { get; set; }
        public EntryData Value { get; set; }
        public int UserInput { get; set; }
        public bool WasChecked { get; set; } = false;
    }
}
