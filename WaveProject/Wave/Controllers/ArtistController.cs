using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wave.Database;
using Wave.Dtos;
using Wave.Models;
using Wave.Validators;

namespace Wave.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ArtistController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly BlobServiceClient _blobService;
        private readonly IOptions<AzureBlobConfig> _config;

        public ArtistController(
            ApplicationDbContext dbContext,
            IMapper mapper,
            BlobServiceClient blobService,
            IOptions<AzureBlobConfig> config)
        {
            _dbContext = dbContext ?? throw new NullReferenceException();
            _mapper = mapper ?? throw new NullReferenceException();
            _blobService = blobService ?? throw new NullReferenceException();
            _config = config ?? throw new NullReferenceException();
        }

        [Authorize(Policy = "read:admin")]
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

        [AllowAnonymous]
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

        [Authorize(Policy = "write:admin")]
        [HttpPost]
        public async Task<IActionResult> CreateArtist([FromBody] CreateArtistDto dto)
        {
            var artist = _mapper.Map<CreateArtistDto, Artist>(dto);
            var validator = new CreateArtistValidator();
            var valRes = await validator.ValidateAsync(dto);
            if (!valRes.IsValid)
                return BadRequest();
            artist.ApplicationUserId = this.User.Identity.Name;
            await _dbContext.Artists.AddAsync(artist);
            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<Artist, ArtistDto>(artist));
        }

        [Authorize(Policy = "modify:admin")]
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

        [Authorize(Policy = "remove:admin")]
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

        [AllowAnonymous]
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpGet("{id}/images/{sId}")]
        public async Task<IActionResult> GetImage([FromRoute] string id, [FromRoute] string sId)
        {
            var img = _blobService.GetBlobContainerClient(_config.Value.ContainerImg).GetBlobClient(sId);
            var res = await img.DownloadAsync();
            return File(res.Value.Content, res.Value.ContentType);
        }

        [Authorize(Policy = "write:admin")]
        [HttpPost("{id}/images")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadArtistImage([FromRoute] string id, IFormFile file)
        {
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest();
            if (file is null || file.Length < 0)
                return BadRequest("file null");
            if (!file.ContentType.StartsWith("image/"))
                return StatusCode(415);

            var artist = await _dbContext.Artists
                .Include(q => q.Image)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();

            if (artist.Image != null)
                _dbContext.ArtistImages.Remove(artist.Image);

            var img = new ArtistImage();
            artist.Image = img;
            await _dbContext.ArtistImages.AddAsync(img);

            var blobContainer = _blobService.GetBlobContainerClient(this._config.Value.ContainerImg);
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blob = blobContainer.GetBlobClient(img.Id);
            await blob.DeleteIfExistsAsync();
            await using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }
            await _dbContext.SaveChangesAsync();
            
            return Ok(_mapper.Map<ImageDto>(img));
        }

        [Authorize(Policy = "remove:admin")]
        [HttpDelete("{id}/images/{sId}")]
        public async Task<IActionResult> RemoveArtistImage(string id, string sId)
        {
            var artist = await _dbContext.Artists
                .Include(q => q.Image)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (artist is null)
                return BadRequest();
            if (artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();

            if (artist.Image?.Id == sId)
            {
                _dbContext.ArtistImages.Remove(artist.Image);
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }

    }
}
