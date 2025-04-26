#pragma warning disable 

namespace BLL2.DTO
{
    public class AudioDto : ContentDto
    {
        public string Artist { get; set; }
        public double Duration { get; set; }
    }
}