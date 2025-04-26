#pragma warning disable 

namespace BLL2.DTO
{
    public class BookDto : ContentDto
    {
        public string Author { get; set; }
        public int PageCount { get; set; }
    }
}