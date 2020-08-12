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
        private readonly IConfiguration _configuration;
        public UserInfoController(IUserInfoService userInfoService, IConfiguration configuration)
        {
            _userInfoService = userInfoService;
            _configuration = configuration;
        }
        // POST api/placeinfo
        [HttpPost]
        public IActionResult PostUserInfo([FromBody]tblUser userinfo)
        {
            string connectionString = _configuration.GetConnectionString("myDb1");
            if (userinfo == null) return BadRequest();
            int retVal = _userInfoService.Add(userinfo, connectionString);
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
    }
}