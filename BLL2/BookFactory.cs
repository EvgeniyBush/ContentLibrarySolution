#pragma warning disable 

using BLL2.DTO;

namespace BLL2.Factories
{
    public class BookFactory : ContentFactory
    {
        private readonly string _title;
        private readonly string _author;
        private readonly int _pages;
        private readonly string _format;

        public BookFactory(string title, string author, int pages, string format)
        {
            _title = title;
            _author = author;
            _pages = pages;
            _format = format;
        }

        public override ContentDto CreateContent()
        {
            return new BookDto
            {
                Title = _title,
                Author = _author,
                PageCount = _pages,
                Format = _format
            };
        }
    }
}
