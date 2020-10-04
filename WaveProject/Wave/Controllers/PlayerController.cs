using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wave.Database;

namespace Wave.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public PlayerController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext ?? throw new NullReferenceException();
            _mapper = mapper ?? throw new NullReferenceException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStream(string id)
        {
            var track = await _dbContext.Tracks
                .Where(q => q.Id == id)
                .Include(q => q.TrackFile)
                .FirstOrDefaultAsync();
            if (track is null)
                return NotFound();



            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpPlayedTrackCounter(string id)
        {
            try
            {
                var track = await _dbContext.Tracks.FindAsync(id);
                if (track is null)
                    return Ok();
                track.Plays++;
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return Ok();
            }
        }
    }
}
