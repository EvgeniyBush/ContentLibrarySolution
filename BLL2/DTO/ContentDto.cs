#pragma warning disable 

namespace BLL2.DTO
{
    public class ContentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Format { get; set; }
        public StorageDto Storage { get; set; }
    }
}
