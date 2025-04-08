using System.ComponentModel.DataAnnotations.Schema;

namespace DAL2.Entities
{
    public class ContentLocation
    {
        public int Id { get; set; }

        public int StorageId { get; set; }

        [ForeignKey(nameof(StorageId))]
        public Storage Storage { get; set; } = null!;

        public int ContentId { get; set; }

        [ForeignKey(nameof(ContentId))]
        public Content Content { get; set; } = null!;
    }
}
