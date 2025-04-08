namespace DAL2.Entities
{
    public class Audio : Content
    {
        public string Artist { get; set; } = string.Empty;
        public double Duration { get; set; } // в хвилинах
    }
}
