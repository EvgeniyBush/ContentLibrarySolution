#pragma warning disable 

using System.ComponentModel.DataAnnotations.Schema;

namespace DAL2.Entities
{
    public class ContentLocation 
    {
        public int Id { get; set; }
        
        [ForeignKey(nameof(StorageId))]
        public int StorageId { get; set; }

        public virtual  Storage Storage { get; set; } = null!;
       
        [ForeignKey(nameof(ContentId))]
        public int ContentId { get; set; }

        public virtual  Content Content { get; set; } = null!;
    }
}
