namespace AnkiLingo.Dtos
{
    public class GetSectionDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public IEnumerable<GetUnitsDto> units { get; set; }
    }

    public class GetUnitsDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public IEnumerable<GetEntryDto> entries { get; set; }
    }

    public class GetEntryDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string value1 { get; set; }
        public string value2 { get; set; }
        public int levelOnKnowledge { get; set; }
    }
}
