using System.Reflection;

namespace GeoComment.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }

        public string? Title { get; set; }

        public string Author { get; set; }
        public double Longitude  { get; set; }

        public double Latitude { get; set; }
        public string Message { get; set; }

        public User User { get; set; }
    }
}
