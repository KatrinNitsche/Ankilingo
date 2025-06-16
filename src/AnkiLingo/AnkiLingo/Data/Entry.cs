namespace AnkiLingo.Data
{
    public class Entry : BaseData
    {
        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }
    
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public int LevelOnKnowledge { get; set; }
        public DateTime LastReviewed { get; set; }
    }
}
