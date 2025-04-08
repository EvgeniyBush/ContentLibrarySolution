namespace DAL2.Entities
{
    public class Document : Content 
    {
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
