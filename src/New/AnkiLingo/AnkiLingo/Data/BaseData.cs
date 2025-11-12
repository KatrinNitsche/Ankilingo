using System.ComponentModel.DataAnnotations;

namespace AnkiLingo.Data
{
    public class BaseData
    {
        [Key] public int Id { get; set; }       
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        [MaxLength(255)] public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
