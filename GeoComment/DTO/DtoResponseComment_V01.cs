namespace GeoComment.Models
{
    public class DtoResponseComment_V01
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Author { get; set; }

        public CommentBody Body { get; set; }
    }
    public class CommentBody
    {
        public string Message { get; set; }
        public string Title { get; set; }

        public string Author { get; set; }
    }
}
