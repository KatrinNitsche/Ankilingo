namespace AnkiLingo.Dtos
{
    public class DetailedCourseDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public IEnumerable<GetSectionDto> Sections { get; set; }
    }
}
