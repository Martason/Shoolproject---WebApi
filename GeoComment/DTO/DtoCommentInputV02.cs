namespace GeoComment.DTO
{
    public class DtoCommentInputV02
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
      

        public CommenInputBodyV2 Body { get; set; }
    }

    public class CommenInputBodyV2
    {
        public string Message { get; set; }
        public string Title { get; set; }

    }



}
