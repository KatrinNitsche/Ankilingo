namespace AnkiLingo.Data
{
    public class ImageEntry : BaseData
    {
        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }
        public string ImagePath { get; set; }      
        public List<EntryPosition> EntryPositions { get; set; } = new List<EntryPosition>();
        public bool IsActive { get; set; }
    }

    public class EntryPosition
    {
        public int Id { get; set; }
        public int EntryId { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
