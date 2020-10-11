using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wave.Database;
using Wave.Dtos;
using Wave.Models;

namespace Wave.Controllers
{
    [Authorize]
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

        [AllowAnonymous]
        [HttpGet("result/{tag}")]
        public async Task<IActionResult> SearchTops(string tag)
        {
            const int maxTaked = 5;
            var artists = await _dbContext.Artists
                .Include(q => q.Image)
                .Where(q => q.Name.Contains(tag)).Take(maxTaked)
                .Select(q => _mapper.Map<Artist, ArtistDto>(q))
                .ToListAsync();
            var albums = await _dbContext.Albums
                .Include(q => q.Image)
                .Where(q => q.Label.Contains(tag)).Take(maxTaked)
                .Select(q => _mapper.Map<Album, AlbumDto>(q))
                .ToListAsync();
            var tracks = await _dbContext.Tracks
                .Include(q => q.Album)
                    .ThenInclude(q => q.Artist)
                .Where(q => q.Title.Contains(tag))
                .Take(maxTaked)
                .Select(q => _mapper.Map<Track, TrackDto>(q))
                .ToListAsync();
            var playlists = await _dbContext.Playlists
                .Include(q => q.Image)
                .Where(q => q.Title.Contains(tag) && (q.ApplicationUserId == this.User.Identity.Name || q.IsPublic))
                .Take(maxTaked)
                .Select(q => _mapper.Map<Playlist, PlaylistDto>(q))
                .ToListAsync();
            return Ok(new { tracks = tracks, albums = albums, artists = artists, playlists = playlists });
        }

        [AllowAnonymous]
        [HttpGet("artists/{tag}")]
        public async Task<IActionResult> SearchArtists(string tag, [FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            const int maxTake = 500;
            if (take > maxTake)
                take = maxTake;
            var artists = await _dbContext.Artists
                .Include(q => q.Image)
                .Where(q => q.Name.Contains(tag))
                .Skip(from)
                .Take(take)
                .Select(q => _mapper.Map<Artist, ArtistDto>(q))
                .ToListAsync();
            return Ok(artists);
        }

        [AllowAnonymous]
        [HttpGet("albums/{tag}")]
        public async Task<IActionResult> SearchAlbums(string tag, [FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            const int maxTake = 500;
            if (take > maxTake)
                take = maxTake;
            var albums = await _dbContext.Albums
                .Include(q => q.Image)
                .Include(q => q.Artist)
                .Where(q => q.Label.Contains(tag))
                .Skip(from)
                .Take(take)
                .Select(q => _mapper.Map<Album, AlbumDto>(q))
                .ToListAsync();
            return Ok(albums);
        }

        [AllowAnonymous]
        [HttpGet("songs/{tag}")]
        public async Task<IActionResult> SearchSongs(string tag, [FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            const int maxTake = 500;
            if (take > maxTake)
                take = maxTake;
            var songs = await _dbContext.Tracks
                .Include(q => q.Album)
                    .ThenInclude(q => q.Artist)
                .Where(q => q.Title.Contains(tag))
                .Skip(from)
                .Take(take)
                .Select(q => _mapper.Map<Track, TrackDto>(q))
                .ToListAsync();
            return Ok(songs);
        }

        [AllowAnonymous]
        [HttpGet("playlists/{tag}")]
        public async Task<IActionResult> SearchPlaylists(string tag, [FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            const int maxTake = 500;
            if (take > maxTake)
                take = maxTake;
            var playlists = await _dbContext.Playlists
                .Include(q => q.Image)
                .Where(q => q.Title.Contains(tag) && (q.IsPublic || q.ApplicationUserId == this.User.Identity.Name))
                .Skip(from)
                .Take(take)
                .Select(q => _mapper.Map<Playlist, PlaylistDto>(q))
                .ToListAsync();
            return Ok(playlists);
        }
    }
}
