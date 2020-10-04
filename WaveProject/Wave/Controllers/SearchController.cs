using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wave.Database;
using Wave.Dtos;
using Wave.Models;

namespace Wave.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public SearchController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext ?? throw new NullReferenceException();
            _mapper = mapper ?? throw new NullReferenceException();
        }

        [HttpGet("result/{tag}")]
        public async Task<IActionResult> SearchTops(string tag)
        {
            return Ok();
        }

        [HttpGet("artists/{tag}")]
        public async Task<IActionResult> SearchArtists(string tag, [FromQuery] int from = 0, [FromQuery] int take = 50)
        {

            return Ok();
        }

        [HttpGet("albums/{tag}")]
        public async Task<IActionResult> SearchAlbums(string tag, [FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            return Ok();
        }

        [HttpGet("songs/{tag}")]
        public async Task<IActionResult> SearchSongs(string tag, [FromQuery] int from = 0, [FromQuery] int take = 50)
        {   
            return Ok();
        }

        [HttpGet("playlists/{tag}")]
        public async Task<IActionResult> SearchPlaylists(string tag, [FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            return Ok();
        }
    }
}
