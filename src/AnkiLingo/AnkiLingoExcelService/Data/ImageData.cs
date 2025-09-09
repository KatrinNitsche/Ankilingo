namespace AnkiLingoExcelService.Data
{
    public class ImageData
    {
        public string SectionName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public List<ImageCover> ImageCovers { get; set; } = new List<ImageCover>();
    }

    public class ImageCover
    {
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public string Value { get; set; } = string.Empty;
        public int LevelOfKnowledge { get; set; }
        public DateTime LastReviewed { get; set; }
        public int ReviewCount { get; set; } = 0;
    }
}
