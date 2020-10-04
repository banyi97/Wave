using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wave.Database;

namespace Wave.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public UploadController(
           ApplicationDbContext dbContext,
           IMapper mapper)
        {
            _dbContext = dbContext ?? throw new NullReferenceException();
            _mapper = mapper ?? throw new NullReferenceException();
        }

        [HttpPost("pic/{type}/{id}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPic(string type, string id, IFormFile file)
        {
            return Ok();
        }

        [HttpDelete("pic/{type}/{id}")]
        public async Task<IActionResult> RemovePic(string type, string id)
        {
            return Ok();
        }

        [HttpPost("track/{id}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadTrack(string id, IFormFile file)
        {
            return Ok();
        }

        [HttpDelete("track/{id}")]
        public async Task<IActionResult> RemoveTrackFile(string id)
        {

            return Ok();
        }
    }
}
