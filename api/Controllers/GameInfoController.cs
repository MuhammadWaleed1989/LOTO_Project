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
    public class GamesInfoController : ControllerBase
    {
        private readonly IGameInfoService _gameInfoService;
        public GamesInfoController(IGameInfoService gameInfoService)
        {
            _gameInfoService = gameInfoService;
        }
        // POST api/gameinfo
        [HttpPost]
        public IActionResult PostGameInfo([FromBody]tblGames gameInfo)
        {

            if (gameInfo == null) return BadRequest();
            int retVal = _gameInfoService.Add(gameInfo);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        // GET api/gameinfo/id
        [HttpGet("{id}", Name = nameof(GetGameInfoById))]
        public IActionResult GetGameInfoById(int id)
        {
            tblGames gameInfo = _gameInfoService.Find(id);
            if (gameInfo == null)
                return NotFound();
            else
                return new ObjectResult(gameInfo);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var games = _gameInfoService.GetAll();
            return Ok(games);
        }

        // DELETE api/placeinfo/5
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int retVal = _gameInfoService.Remove(id);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        // PUT api/GameInfo/guid
        [HttpPut("{id}")]
        public IActionResult PutGameInfo(int id, [FromBody] tblGames gameInfo)
        {
            if (gameInfo == null || id != gameInfo.GameID) return BadRequest();
            if (_gameInfoService.Find(id) == null) return NotFound();
            int retVal = _gameInfoService.Update(id,gameInfo);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        [Authorize]
        [HttpPost("UpdateWinnigValues")]
        public IActionResult UpdateWinnigValues([FromBody] tblGames gameInfo)
        {
            if (_gameInfoService.Find(gameInfo.GameID) == null) return NotFound();
            int retVal = _gameInfoService.UpdateWinnigValues(gameInfo);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        [Authorize]
        [HttpPost("InsertUserOfGame")]
        public IActionResult InsertUserOfGame([FromBody]tblUserOfGame usersOfGame)
        {

            if (usersOfGame == null) return BadRequest();
            int retVal = _gameInfoService.InsertUserOfGame(usersOfGame);
            if (retVal > 0) return Ok(); else return NotFound();
        }
    }
}