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
    public class UserGameController : ControllerBase
    {
        private readonly IUserGameService _userGameService;
        public UserGameController(IUserGameService userGameService)
        {
            _userGameService = userGameService;
        }
        // POST api/gameinfo
        [HttpPost]
        public IActionResult PostGameInfo([FromBody]tblUserGame userGamedata)
        {
           
            if (userGamedata == null) return BadRequest();
            int retVal = _userGameService.Add(userGamedata);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        // GET api/gameinfo/id
        [HttpGet("{id}", Name = nameof(GetUserGameById))]
        public IActionResult GetUserGameById(int id)
        {
            tblUserGame userGamedata = _userGameService.Find(id);
            if (userGamedata == null)
                return NotFound();
            else
                return new ObjectResult(userGamedata);
        }
        [HttpPut("{id}")]
        public IActionResult PostGameInfo(int id, [FromBody] tblUserGame userGamedata)
        {
            if (userGamedata == null || id != userGamedata.GameID) return BadRequest();
            if (_userGameService.Find(id) == null) return NotFound();
            int retVal = _userGameService.Update(id, userGamedata);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var games = _userGameService.GetAll();
            return Ok(games);
        }
    }
}