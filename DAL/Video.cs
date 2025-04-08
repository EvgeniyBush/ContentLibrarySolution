namespace DAL2.Entities
{
    public class Video : Content
    {
        public string Director { get; set; } = string.Empty;
        public int ResolutionHeight { get; set; }
    }
}

