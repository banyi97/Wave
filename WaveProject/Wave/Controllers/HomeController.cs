using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wave.Database;
using Wave.Dtos;
using Wave.Models;

namespace Wave.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public HomeController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext ?? throw new NullReferenceException();
            _mapper = mapper ?? throw new NullReferenceException();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetHome()
        {
            const int take = 20;

            var newAlbums = await _dbContext.Albums
                .OrderByDescending(q => q.ReleaseDate)
                .Take(take)
                .Include(q => q.Image)
                .Include(q => q.Artist)
                .Select(q => _mapper.Map<AlbumDto>(q))
                .ToListAsync();

            return Ok(newAlbums);
        }

        [HttpGet("yourlib")]
        public async Task<IActionResult> GetYourLib()
        {
            var res = await _dbContext.Playlists
                .Where(q => q.ApplicationUserId == this.User.Identity.Name)
                .Include(q => q.Image)
                .Select(q => _mapper.Map<PlaylistDto>(q))
                .ToListAsync();

            return Ok(res);
        }
    }
}
