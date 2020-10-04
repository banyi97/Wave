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
    public class ArtistController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ArtistController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext ?? throw new NullReferenceException();
            _mapper = mapper ?? throw new NullReferenceException();
        }

        [HttpGet]
        public async Task<IActionResult> GetArtists([FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            const int maxTake = 500;
            if (take > maxTake)
                take = maxTake;
            var artists = await _dbContext.Artists
                .Include(q => q.Image)
                .Skip(from)
                .Take(take)
                .Select(q => _mapper.Map<Artist, ArtistDto>(q))
                .ToListAsync();
            return Ok(artists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArtist([FromRoute] string id)
        {
            var artist = await _dbContext.Artists
                .Where(q => q.Id == id)
                .Include(q => q.Image)
                .Select(q => _mapper.Map<Artist, ArtistDto>(q))
                .SingleOrDefaultAsync();
            if (artist is null)
                return NotFound();
            return Ok(artist);
        }

        [HttpPost]
        public async Task<IActionResult> CreateArtist([FromBody] CreateArtistDto dto)
        {
            var artist = _mapper.Map<CreateArtistDto, Artist>(dto);
            artist.ApplicationUserId = this.User.Identity.Name;
            await _dbContext.Artists.AddAsync(artist);
            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<Artist, ArtistDto>(artist));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyArtist(string id, [FromBody] ArtistDto dto)
        {
            var artist = await _dbContext.Artists
                .Where(q => q.Id == id)
                .SingleOrDefaultAsync();
            if (artist is null)
                return NotFound();
            if (artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();

            artist.Name = dto.Name;
            artist.Description = dto.Description;

            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<Artist, ArtistDto>(artist));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveArtist(string id)
        {
            var artist = await _dbContext.Artists
                .Include(q => q.Image)
                .Include(q => q.Albums)
                    .ThenInclude(q => q.Image)
                .Include(q => q.Albums)
                    .ThenInclude(q => q.Tracks)
                        .ThenInclude(q => q.TrackFile)
                .Where(q => q.Id == id)
                .SingleOrDefaultAsync();

            if (artist is null)
                return Ok();
            if (artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            _dbContext.Artists.Remove(artist);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}/top")]
        public async Task<IActionResult> GetTopTracks([FromRoute] string id)
        {
            const int maxTopSong = 10;
            var tracks = await _dbContext.Tracks
                    .Include(q => q.Album)
                        .ThenInclude(q => q.Image)
                    .Include(q => q.TrackFile)
                    .Where(q => q.Album.ArtistId == id)
                    .OrderBy(q => q.Plays)
                    .Take(maxTopSong)
                    .Select(q => _mapper.Map<Track, TrackDto>(q))
                    .ToListAsync();
            return Ok(tracks);
        }

        [HttpGet("{id}/albums")]
        public async Task<IActionResult> GetAlbums(string id)
        {
            var albums = await _dbContext.Albums
                    .Include(q => q.Tracks)
                        .ThenInclude(q => q.TrackFile)
                    .Include(q => q.Image)
                    .Include(q => q.Artist)
                    .Where(q => q.ArtistId == id)
                    .OrderByDescending(q => q.ReleaseDate)
                    .ThenByDescending(q => q.CreatedDate)
                    .Select(q => _mapper.Map<Album, AlbumDto>(q))
                    .ToListAsync();
            //foreach (var item in albums)
            //{
            //    item.Tracks = item.Tracks.OrderBy(q => q.DiscNumber).ThenBy(q => q.NumberOf).ToList();
            //}
            return Ok(albums);
        }
    }
}
