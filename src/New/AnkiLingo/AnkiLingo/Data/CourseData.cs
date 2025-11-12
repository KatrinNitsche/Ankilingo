namespace AnkiLingoExcelService.Data
{
    public class CourseData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        public List<SectionData> Sections { get; set; } = new List<SectionData>();
    }

    public class SectionData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<UnitData> Units { get; set; } = new List<UnitData>();
    }

    public class UnitData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EntryData> Entries { get; set; } = new List<EntryData>();
        public string BackgroundColor { get; set; } = string.Empty;
    }

    public class EntryData
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public int LevelOfKnowledge { get; set; }
        public DateTime LastReviewed { get; set; }
        public int ReviewCount { get; set; } = 0;
    }
}
