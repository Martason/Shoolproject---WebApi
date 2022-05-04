using GeoComment.DTO;
using GeoComment.Models;
using GeoComment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("api/user")]
    [ApiController]
    [ApiVersion("0.2")]
    public class UserController : ControllerBase
    {

        private readonly GeoUserService _geoUserService;


        public UserController(GeoUserService geoUserService)
        {
            _geoUserService = geoUserService;
        }

        [Route("register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseUser>> RegisterUser(NewUserDto newUser)
        {
            var user = await _geoUserService.RegisterNewUser(newUser);

            if (user == null) return BadRequest();

            var responseUser = new ResponseUser
            {
                Id = user.Id,
                Username = user.UserName
            };

            return Created("",responseUser);

        }

        [Route("login")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login(NewUserDto userToLogin)
        {
            var tokenStr = await _geoUserService.Login(userToLogin);
            if (tokenStr == null) return BadRequest();

            var token = new {token = tokenStr};

            return Ok(token);
        }



    }
}
