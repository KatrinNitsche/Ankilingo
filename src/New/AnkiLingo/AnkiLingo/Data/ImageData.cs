using AnkiLingoExcelService.Data;

namespace AnkiLingo.Data
{
    public class ImageData
    {
        public Guid id { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public List<ImageWord> ImageCovers { get; set; } = new List<ImageWord>();
    }

    public class ImageWord
    {
        public Guid id { get; set; }
        public int EntryId { get; set; }
        public EntryData Value { get; set; }
        public int UserInput { get; set; }
        public bool WasChecked { get; set; } = false;
    }
}
