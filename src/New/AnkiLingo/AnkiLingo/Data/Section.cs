namespace AnkiLingo.Data
{
    public class Section : BaseData
    {
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Unit> Units { get; set; }
        public int Order { get; set; }
    }
}
