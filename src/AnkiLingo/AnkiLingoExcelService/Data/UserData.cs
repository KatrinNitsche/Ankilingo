namespace AnkiLingoExcelService.Data
{
    public class UserData
    {
        public int StreakLength { get; set; }
        public int GemsCount { get; set; }
        public string CurrentCourse { get; set; }
        public DateTime LastStudy { get; set; } = DateTime.MinValue;
    }
}
