using System.ComponentModel.DataAnnotations;

namespace DAL2.Entities
{
    public class Storage 
    {
        [Key]
        public int Id { get; set; }

        public string LocationName { get; set; } = string.Empty;

        public ICollection<ContentLocation> ContentLocations { get; set; } = new List<ContentLocation>();
    }
}
