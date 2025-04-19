#pragma warning disable 

using System.ComponentModel.DataAnnotations;

namespace DAL2.Entities
{
    public abstract class Content 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Format { get; set; } = string.Empty;

        public virtual  ContentLocation? Location { get; set; }
    }
}