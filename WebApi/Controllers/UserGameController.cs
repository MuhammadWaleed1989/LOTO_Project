using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;
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
        public IActionResult PostGameInfo([FromBody]GameData gamedata)
        {
           
            if (gamedata == null) return BadRequest();
            int retVal = _gameInfoService.Add(gamedata);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        // GET api/gameinfo/id
        [HttpGet("{id}", Name = nameof(GetGameInfoById))]
        public IActionResult GetGameInfoById(int id)
        {
            GameData gamedata = _gameInfoService.Find(id);
            if (gamedata == null)
                return NotFound();
            else
                return new ObjectResult(gamedata);
        }
        //[HttpPut("{id}")]
        //public IActionResult PostGameInfo(int id, [FromBody] tblGames gameInfo)
        //{
        //    if (gameInfo == null || id != gameInfo.GameID) return BadRequest();
        //    if (_gameInfoService.Find(id) == null) return NotFound();
        //    int retVal = _gameInfoService.Update(id, gameInfo);
        //    if (retVal > 0) return Ok(); else return NotFound();
        //}
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var games = _gameInfoService.GetAll();
            return Ok(games);
        }
    }
}