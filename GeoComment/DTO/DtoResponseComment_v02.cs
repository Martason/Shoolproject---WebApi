using GeoComment.Models;

namespace GeoComment.DTO
{
    public class DtoResponseComment_v02
    {
        public static DtoResponseComment_v02 CreateResponseComment_v02(Comment comment)
        {
            
            var responseComment = new DtoResponseComment_v02
            {
                Id = comment.Id,
                Longitude = comment.Longitude,
                Latitude = comment.Latitude,

                Body = new ResponseCommentBody
                {
                    Message = comment.Message,
                    Title = comment.Title,
                    Author = comment.Author,
                }
            }; 
            
            return responseComment;

        }

        public int Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public ResponseCommentBody Body { get; set; }

    }

    public class ResponseCommentBody
    {
        public string Message { get; set; }
        public string Title { get; set; }

        public string Author { get; set; }

    }
}
