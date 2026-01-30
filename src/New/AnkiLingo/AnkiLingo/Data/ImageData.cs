using AnkiLingoExcelService.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnkiLingo.Data
{
    public class ImageData
    {
        [Key] public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public List<ImageWord> ImageWords { get; set; } = new List<ImageWord>();
    }

    public class ImageWord
    {
        [Key] public Guid Id { get; set; }
        public Guid ImageId { get; set; }     
        public int EntryId { get; set; }
        public string EntryText { get; set; } = string.Empty;

        [NotMapped] public bool WasChecked { get; set; }
        [NotMapped] public int UserInput { get; set; }
        [NotMapped] public EntryData? EntryData { get; set; }
    }
}
