using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWebApi.Models;
using TestWebApi.Services;
using Microsoft.Extensions.Configuration;

namespace TestWebApi.Controllers
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
        // POST api/placeinfo
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
    }
}