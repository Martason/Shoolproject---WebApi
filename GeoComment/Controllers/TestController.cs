﻿using GeoComment.Services.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly DatabaseHandler _databaseHandler;
        private readonly IHostEnvironment _environment;

        public TestController(DatabaseHandler databaseHandler, IHostEnvironment environment)
        {
            _databaseHandler = databaseHandler;
            _environment = environment;
        }

        [HttpGet]
        [Route("reset-db")]//detta heter operation del av open API
        [ApiVersion("0.1")]
        [ApiVersion("0.2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetDatabase()
        {
            if (_environment.IsDevelopment())
                try
                {
                    await _databaseHandler.Recreate();
                    return Ok();
                }

                catch (Exception)
                {
                    return StatusCode(500);
                }

            return StatusCode(404);
        }

    }
}
