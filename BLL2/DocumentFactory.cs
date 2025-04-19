#pragma warning disable 

using DAL2.Entities;

namespace BLL2.Factories
{
    public class DocumentFactory : ContentFactory 
    {
        private readonly string _title;
        private readonly string _author;
        private readonly string _description;
        private readonly string _format;

        public DocumentFactory(string title, string author, string description, string format)
        {
            _title = title;
            _author = author;
            _description = description;
            _format = format;
        }

        public override Content CreateContent()
        {
            return new Document
            {
                Title = _title,
                Author = _author,
                Description = _description,
                Format = _format
            };
        }
    }
}
