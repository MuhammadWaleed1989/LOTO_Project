using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http.Headers;

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
        [HttpGet("GetLastWinner")]
        public IActionResult GetLastWinner()
        {
            var games = _gameInfoService.GetLastWinner();
            return Ok(games);
        }
        // POST api/gameinfo
        [Authorize]
        [HttpPost]
        public IActionResult PostGameInfo([FromBody]tblGames gameInfo)
        {

            if (gameInfo == null) return BadRequest();
            int retVal = _gameInfoService.Add(gameInfo);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        // GET api/gameinfo/id
        [Authorize]
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
        [HttpPost("Upload"), DisableRequestSizeLimit]
        public IActionResult Upload([FromForm] int gameID)
        {
            try
            {
                var file = Request.Form.Files[0];
                
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    int retVal = _gameInfoService.UpdateWinnerImage(dbPath, gameID);
                    
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}