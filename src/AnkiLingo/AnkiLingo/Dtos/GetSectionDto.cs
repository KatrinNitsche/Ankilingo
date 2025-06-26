namespace AnkiLingo.Dtos
{
    public class GetSectionDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public IEnumerable<GetUnitsDto> Units { get; set; }
    }

    public class GetUnitsDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public IEnumerable<GetEntryDto> Entries { get; set; }
    }

    public class GetEntryDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public int LevelOnKnowledge { get; set; }
        public DateTime LastReviewed { get; set; }
    }
}
