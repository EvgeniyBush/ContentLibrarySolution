#pragma warning disable 

using BLL2.DTO;

namespace BLL2.Factories
{
    public class VideoFactory : ContentFactory 
    {
        private readonly string _title;
        private readonly string _director;
        private readonly int _resolution;
        private readonly string _format;

        public VideoFactory(string title, string director, int resolution, string format)
        {
            _title = title;
            _director = director;
            _resolution = resolution;
            _format = format;
        }

        public override ContentDto CreateContent()
        {
            return new VideoDto
            {
                Title = _title,
                Director = _director,
                ResolutionHeight = _resolution,
                Format = _format
            };
        }
    }
}
