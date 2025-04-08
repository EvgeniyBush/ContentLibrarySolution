namespace DAL2.Entities
{
    public class Book : Content 
    {
        public string Author { get; set; } = string.Empty;
        public int PageCount { get; set; }
    }
}
