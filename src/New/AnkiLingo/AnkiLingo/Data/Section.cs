namespace AnkiLingo.Data
{
    public class Section : BaseData
    {
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Unit> Units { get; set; }

    }
}
