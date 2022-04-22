namespace GeoComment.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
    }
}
