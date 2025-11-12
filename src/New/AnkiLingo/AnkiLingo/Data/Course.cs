namespace AnkiLingo.Data
{
    public class Course : BaseData
    {
        public string? Icon { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
    }
}
