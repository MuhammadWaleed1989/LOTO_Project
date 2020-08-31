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
    public class UserInfoController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        public UserInfoController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }
        // POST api/userinfo
        [HttpPost]
        public IActionResult PostUserInfo([FromBody]tblUser userinfo)
        {
           
            if (userinfo == null) return BadRequest();
            int retVal = _userInfoService.Add(userinfo);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userInfoService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }
        // GET api/userinfo/id
        [HttpGet("{id}", Name = nameof(GetUserInfoById))]
        public IActionResult GetUserInfoById(int id)
        {
            tblUser userInfo = _userInfoService.Find(id);
            if (userInfo == null)
                return NotFound();
            else
                return new ObjectResult(userInfo);
        }
        [HttpPut("{id}")]
        public IActionResult PostUserInfo(int id, [FromBody] tblUser userinfo)
        {
            if (userinfo == null || id != userinfo.UserID) return BadRequest();
            if (_userInfoService.Find(id) == null) return NotFound();
            int retVal = _userInfoService.UpdateOnlineStatus(userinfo.UserID, false);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userInfoService.GetAll();
            return Ok(users);
        }
        [Authorize]
        [HttpPost("UpdatedCoinsDetail")]
        public IActionResult UpdatedCoinsDetail([FromBody] tblUser userInfo)
        {
            if (_userInfoService.Find(userInfo.UserID) == null) return NotFound();
            int retVal = _userInfoService.Update(userInfo.UserID, userInfo);
            if (retVal > 0) return Ok(); else return NotFound();
        }
    }
}