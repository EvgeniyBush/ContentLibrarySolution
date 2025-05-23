﻿#pragma warning disable 

using BLL2.DTO;

namespace BLL2.Factories
{
    public class AudioFactory : ContentFactory 
    {
        private readonly string _title;
        private readonly string _artist;
        private readonly double _duration;
        private readonly string _format;

        public AudioFactory(string title, string artist, double duration, string format)
        {
            _title = title;
            _artist = artist;
            _duration = duration;
            _format = format;
        }

        public override ContentDto CreateContent()
        {
            return new AudioDto
            {
                Title = _title,
                Artist = _artist,
                Duration = _duration,
                Format = _format
            };
        }
    }
}
