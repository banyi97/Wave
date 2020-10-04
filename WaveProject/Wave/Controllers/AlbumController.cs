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
    public class AlbumController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public AlbumController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext ?? throw new NullReferenceException();
            _mapper = mapper ?? throw new NullReferenceException();
        }

        [HttpGet]
        public async Task<IActionResult> GetAlbums([FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            const int maxTake = 500;
            if (take > maxTake)
                take = maxTake;
            var albums = await _dbContext.Albums
                    .Include(q => q.Artist)
                    .Skip(from)
                    .Take(take)
                    .Select(q => _mapper.Map<Album, AlbumDto>(q))
                    .ToListAsync();
            return Ok(albums);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbum([FromRoute] string id)
        {
            var album = await _dbContext.Albums
                    .Include(q => q.Tracks)
                        .ThenInclude(q => q.TrackFile)
                    .Include(q => q.Artist)
                    .Include(q => q.Image)
                    .Where(q => q.Id == id)
                    .Select(q => _mapper.Map<Album, AlbumDto>(q))
                    .SingleOrDefaultAsync();
            if (album is null)
                return NotFound();
            //album.Tracks = album.Tracks.OrderBy(q => q.DiscNumber).ThenBy(q => q.NumberOf).ToList();
            return Ok(album);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlbum([FromBody] CreateAlbumDto dto)
        {
            var artist = await _dbContext.Artists.FindAsync(dto.ArtistId);
            if (artist is null)
                return BadRequest();

            if (artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            var album = _mapper.Map<CreateAlbumDto, Album>(dto);
            await _dbContext.Albums.AddAsync(album);
            var numberOf = 0;
            var tracks = new List<Track>(dto.Tracks.Select(q =>
            {
                q.AlbumId = album.Id;
                q.NumberOf = numberOf++;
                return _mapper.Map<CreateTrackDto, Track>(q);
            }));
            await _dbContext.Tracks.AddRangeAsync(tracks);

            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<Album, AlbumDto>(album));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyAlbum([FromRoute] string id, [FromBody] AlbumDto dto)
        {
            var album = await _dbContext.Albums
                .Include(q => q.Artist)
                .Where(q => q.Id == id)
                .SingleOrDefaultAsync();
            if (album is null)
                return NotFound();

            if (album.Artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();

            album.Label = dto.Label;

            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<Album, AlbumDto>(album));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAlbum([FromRoute] string id)
        {
            var album = await _dbContext.Albums
                .Include(q => q.Artist)
                .Include(q => q.Image)
                .Include(q => q.Tracks)
                    .ThenInclude(q => q.TrackFile)
                .Where(q => q.Id == id)
                .SingleOrDefaultAsync();
            if (album is null)
                return Ok();
            if (album.Artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();

            _dbContext.Albums.Remove(album);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}/tracks")]
        public async Task<IActionResult> GetTracks([FromRoute] string id)
        {
            var album = await _dbContext.Albums.FindAsync(id);
            if (album is null)
                return NotFound();
            var songs = await _dbContext.Tracks
                    .Include(q => q.TrackFile)
                    .Include(q => q.Album)
                        .ThenInclude(q => q.Artist)
                    .Where(q => q.AlbumId == id)
                    .OrderBy(q => q.DiscNumber)
                    .ThenBy(q => q.NumberOf)
                    .Select(q => _mapper.Map<Track, TrackDto>(q))
                    .ToListAsync();
            return Ok(songs);
        }

        [HttpPost("{id}/tracks")]
        public async Task<IActionResult> AddTrack([FromRoute] string id, [FromBody] CreateTrackDto dto)
        {
            var album = await _dbContext.Albums
                .Include(q => q.Tracks)
                .Include(q => q.Artist)
                .Where(q => q.Id == id)
                .SingleOrDefaultAsync();
            if (album is null)
                return NotFound();
            if (album.Artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            var albumTracks = album.Tracks
                .Where(q => q.DiscNumber == dto.DiscNumber)
                .OrderBy(q => q.NumberOf)
                .ToList();

            // add

            await _dbContext.SaveChangesAsync();
            return Ok(albumTracks.Select(q => _mapper.Map<Track, TrackDto>(q)));
        }

        [HttpPut("{id}/tracks")]
        public async Task<IActionResult> ModifyTrack([FromRoute] string id, [FromBody] TrackDto dto)
        {           
            var track = await _dbContext.Tracks
                .Where(q => q.AlbumId == id && q.Id == dto.Id)
                .Include(q => q.Album)
                    .ThenInclude(q => q.Artist)
                .SingleOrDefaultAsync();
            if (track is null)
                return NotFound();
            if (track.Album.Artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            track.Title = dto.Title;
            track.IsExplicit = dto.IsExplicit;

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}/tracks/{sId}")]
        public async Task<IActionResult> RemoveTrack([FromRoute] string id, [FromRoute] string sId)
        {
            var track = await _dbContext.Tracks
                .Where(q => q.AlbumId == id && q.Id == sId)
                .Include(q => q.Album)
                    .ThenInclude(q => q.Artist)
                .SingleOrDefaultAsync();
            if (track is null)
                return Ok();
            if (track.Album.Artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            var albumTracks = await _dbContext.Tracks
                .Where(q => q.AlbumId == track.AlbumId && q.DiscNumber == track.DiscNumber)
                .OrderBy(q => q.NumberOf)
                .ToListAsync();

            albumTracks.Remove(track);
            _dbContext.Tracks.Remove(track);

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
