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
    public class AdminConfigController : ControllerBase
    {
        private readonly IAdminConfigService _configInfoService;
        public AdminConfigController(IAdminConfigService configInfoService)
        {
            _configInfoService = configInfoService;
        }
        // POST api/userinfo
        [Authorize]
        [HttpPost]
        public IActionResult PostAdminConfig([FromBody]tblAdminConfig configinfo)
        {
           
            if (configinfo == null) return BadRequest();
            int retVal = _configInfoService.Add(configinfo);
            if (retVal > 0) return Ok(); else return NotFound();
        }

        [Authorize]
        [HttpGet("{id}", Name = nameof(GetAdminConfigById))]
        public IActionResult GetAdminConfigById(int id)
        {
            tblAdminConfig configInfo = _configInfoService.Find(id);
            if (configInfo == null)
                return NotFound();
            else
                return new ObjectResult(configInfo);
        }
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult PostAdminConfig(int id, [FromBody] tblAdminConfig configinfo)
        {
            if (configinfo == null || id != configinfo.Adminid) return BadRequest();
            if (_configInfoService.Find(id) == null) return NotFound();
            int retVal = _configInfoService.Update(configinfo.Adminid, configinfo);
            if (retVal > 0) return Ok(); else return NotFound();
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _configInfoService.GetAll();
            return Ok(users);
        }
    }
}