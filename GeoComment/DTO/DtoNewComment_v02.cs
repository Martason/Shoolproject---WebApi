namespace GeoComment.DTO
{
    public class DtoNewComment_v02
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
      

        public  NewCommentBodyV2 Body { get; set; }
    }

    public class NewCommentBodyV2
    {
        public string Message { get; set; }
        public string Title { get; set; }

    }



}
