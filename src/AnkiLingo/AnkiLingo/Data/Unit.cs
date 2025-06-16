namespace AnkiLingo.Data
{
    public class Unit : BaseData
    {        
        public int SectionId { get; set; }
        public virtual Section Section { get; set; }
        public virtual ICollection<Entry> Entries { get; set; }
    }
}
