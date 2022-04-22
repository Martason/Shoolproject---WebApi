using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("api/geo-comments")]
    [ApiController]
    public class GeoCommentsController : ControllerBase
    {
        //forma input och output av data för att passa

        public class NewComment //DTO data transfer object. Används bara för att föra över information från ett ställe till en annan
        {
            public string Message { get; set; }
            public int Longitude { get; set; }
            public int Latitude { get; set; }
        }

        public class PartiallyHiddenComment //DTO
        {

            public string Message { get; set; }
            public int Longitude { get; set; }
            public int Latitude { get; set; }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<NewComment>> CreateNewComment(NewComment newComment)
        {
            if (newComment == null) return BadRequest();

            var _newComment = new NewComment
            {
                Message = newComment.Message,
                Longitude = newComment.Longitude,
                Latitude = newComment.Latitude,
            };



            return StatusCode(500);
        }




    }
}
