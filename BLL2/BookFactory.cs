using DAL2.Entities;

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

        public override Content CreateContent()
        {
            return new Book
            {
                Title = _title,
                Author = _author,
                PageCount = _pages,
                Format = _format
            };
        }
    }
}
