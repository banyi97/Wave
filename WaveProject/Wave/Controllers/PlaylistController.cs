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
    public class PlaylistController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public PlaylistController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext ?? throw new NullReferenceException();
            _mapper = mapper ?? throw new NullReferenceException();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlaylist([FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            const int maxTake = 500;
            if (take > maxTake)
                take = maxTake;
            var playlists = await _dbContext.Playlists
                    .Skip(from)
                    .Take(take)
                    .Select(q => _mapper.Map<Playlist, PlaylistDto>(q))
                    .ToListAsync();
            return Ok(playlists);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyAllPlaylist()
        {
            var playlists = await _dbContext.Playlists
                    .Where(q => q.ApplicationUserId == this.User.Identity.Name)
                    .OrderBy(q => q.NumberOf)
                    .ThenByDescending(q => q.LatestUpdate)
                    .Select(q => _mapper.Map<Playlist, PlaylistDto>(q))
                    .ToListAsync();
            foreach (var item in playlists)
                item.IsMy = true;
            return Ok(playlists);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserAllPublicPlaylist(string id)
        {
            var playlists = await _dbContext.Playlists
                    .Where(q => q.ApplicationUserId == id && q.IsPublic)
                    .Include(q => q.Image)
                    .Select(q => _mapper.Map<Playlist, PlaylistDto>(q))
                    .ToListAsync();
            return Ok(playlists);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistDto dto)
        {
            const int maxPlaylistNumber = 50;
            var playlists = await _dbContext.Playlists
                .Where(q => q.ApplicationUserId == this.User.Identity.Name)
                .OrderBy(q => q.NumberOf)
                .ThenByDescending(q => q.LatestUpdate)
                .ToArrayAsync();
            if (playlists.Length > maxPlaylistNumber)
                return BadRequest("You have too many playlist!");
            var playlist = _mapper.Map<CreatePlaylistDto, Playlist>(dto);
            playlist.ApplicationUserId = this.User.Identity.Name;
            playlist.NumberOf = playlists.LastOrDefault() == null ? 0 : playlists.Last().NumberOf + 1;
            await _dbContext.Playlists.AddAsync(playlist);
            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<Playlist, PlaylistDto>(playlist));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ReorderPlaylist(string id, [FromQuery] int from, [FromQuery] int to)
        {
            
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylist(string id)
        {
            var playlist = await _dbContext.Playlists
                    .Include(q => q.Image)
                    .Where(q => q.Id == id)
                    .SingleOrDefaultAsync();
            if (playlist is null)
                return NotFound();

            if (playlist.ApplicationUserId == this.User.Identity.Name || playlist.IsPublic)
            {
                playlist.PlaylistElements = await _dbContext.PlaylistElements
                    .Where(q => q.PlayListId == id)
                    .Include(q => q.Track)
                        .ThenInclude(q => q.TrackFile)
                    .Include(q => q.Track)
                        .ThenInclude(q => q.Album)
                            .ThenInclude(q => q.Artist)
                    .Include(q => q.Track)
                        .ThenInclude(q => q.Album.Image)
                    .OrderBy(q => q.NumberOf)
                    .ToListAsync();
                var ret = _mapper.Map<Playlist, PlaylistDto>(playlist);
                if (playlist.ApplicationUserId == this.User.Identity.Name)
                    ret.IsMy = true; // for drag and drop
                return Ok(ret);
            }
            return Forbid();
        }

        [HttpPatch("{id}/rename")]
        public async Task<IActionResult> RenamePlaylist(string id, [FromBody] PlaylistDto dto)
        {
            var playlist = await _dbContext.Playlists.FindAsync(id);
            if (playlist is null)
                return NotFound();
            if (playlist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();

            playlist.Title = dto.Title;

            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<Playlist, PlaylistDto>(playlist));
        }

        [HttpPatch("{id}/public")]
        public async Task<IActionResult> MakePublicPlaylist(string id, [FromBody] PlaylistDto dto)
        {
            var playlist = await _dbContext.Playlists.FindAsync(id);
            if (playlist is null)
                return NotFound();
            if (playlist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            playlist.IsPublic = dto.IsPublic;
            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<Playlist, PlaylistDto>(playlist));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemovePlaylist(string id)
        {
            return Ok();
        }

        [HttpPost("{id}/{sId}")]
        public async Task<IActionResult> AddToPlaylist(string id, string sId)
        {
            return Ok();
        }

        [HttpPut("{id}/{sId}")]
        public async Task<IActionResult> ReOrderPlaylist(string id, string sId, [FromQuery] int next)
        {

            return Ok();
        }

        [HttpDelete("{id}/{sId}")]
        public async Task<IActionResult> RemoveFromPlaylist(string id, string sId)
        {
          
            return Ok();
        }
    }
}
