using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserGameController : ControllerBase
    {
        private readonly IUserGameService _userGameService;
        public UserGameController(IUserGameService userGameService)
        {
            _userGameService = userGameService;
        }

        [Authorize]
        [HttpGet("GetGameValues")]
        public IActionResult GetGameValues()
        {
            var games = _userGameService.GetGameValues();
            return Ok(games);
        }



        // POST api/gameinfo
        [HttpPost]
        public IActionResult PostUserGame([FromBody]tblUserGame userGamedata)
        {

            if (userGamedata == null) return BadRequest();
            int retVal = _userGameService.Add(userGamedata);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        // GET api/gameinfo/id
        [HttpGet("{gameID}", Name = nameof(GetUserGameById))]
        public IActionResult GetUserGameById(int gameID)
        {
            tblUserGame userGamedata = _userGameService.Find(gameID);
            if (userGamedata == null)
                return NotFound();
            else
                return new ObjectResult(userGamedata);
        }
        
        [Authorize]
        [HttpPost("GetAllValue")]
        public IActionResult GetAllValue(int gameID)
        {
            var games = _userGameService.GetAllValue(gameID);
            return Ok(games);
        }
        [Authorize]
        [HttpPost("ConfirmedUserGameValues")]
        public IActionResult ConfirmedUserGameValues([FromBody]List<tblUserGame> userGamedata)
        {

            if (userGamedata == null) return BadRequest();
            int retVal = _userGameService.Update(userGamedata);
            if (retVal > 0) return Ok(); else return NotFound();
        }

        [Authorize]
        [HttpPost("UpdateGameStatus")]
        public IActionResult UpdateGameStatus([FromBody] tblGames gameInfo)
        {

            if (gameInfo == null) return BadRequest();
            int retVal = _userGameService.Update(gameInfo.GameID, gameInfo);
            if (retVal > 0) return Ok(); else return NotFound();
        }
    }
}