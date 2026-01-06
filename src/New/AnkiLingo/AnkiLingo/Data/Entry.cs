namespace AnkiLingo.Data
{
    public class Entry : BaseData
    {
        public virtual Unit Unit { get; set; }    
        public required string Value1 { get; set; }
        public required string Value2 { get; set; }
    }
}
