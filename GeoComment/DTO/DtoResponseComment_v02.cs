namespace GeoComment.DTO
{
    public class DtoResponseComment_v02
    {
        public int Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public ResponseCommentBodyV2 Body { get; set; }

    }

    public class ResponseCommentBodyV2
    {
        public string Message { get; set; }
        public string Title { get; set; }

        public string Author { get; set; }

    }
}
