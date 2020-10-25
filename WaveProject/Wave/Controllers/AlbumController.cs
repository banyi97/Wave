using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NAudio.Wave;
using Wave.Database;
using Wave.Dtos;
using Wave.Models;
using Wave.Services;

namespace Wave.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AlbumController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly BlobServiceClient _blobService;
        private readonly IOptions<AzureBlobConfig> _config;

        public AlbumController(
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

        [AllowAnonymous]
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

        [Authorize(Policy = "write:admin")]
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

        [Authorize(Policy = "modify:admin")]
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

        [Authorize(Policy = "remove:admin")]
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

        [AllowAnonymous]
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

        [Authorize(Policy = "write:admin")]
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

            var track = _mapper.Map<CreateTrackDto, Track>(dto);
            track.AlbumId = id;
            await _dbContext.Tracks.AddAsync(track);
            if (dto.NumberOf >= albumTracks.Count)
                albumTracks.Add(track);
            else
                albumTracks.Insert(dto.NumberOf, track);

            albumTracks.Renumber();

            await _dbContext.SaveChangesAsync();
            return Ok(albumTracks.Select(q => _mapper.Map<Track, TrackDto>(q)));
        }

        [Authorize(Policy = "modify:admin")]
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

        [Authorize(Policy = "remove:admin")]
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

        [Authorize(Policy = "write:admin")]
        [HttpPost("{id}/images")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAlbumImage([FromRoute] string id, IFormFile file)
        {
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest();
            if (file is null || file.Length < 0)
                return BadRequest("file null");
            if (!file.ContentType.StartsWith("image/"))
                return StatusCode(415);

            var album = await _dbContext.Albums
                .Include(q => q.Image)
                .Include(q => q.Artist)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (album.Artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();

            if (album.Image != null)
                _dbContext.AlbumImages.Remove(album.Image);

            var img = new AlbumImage();
            album.Image = img;
            await _dbContext.AlbumImages.AddAsync(img);

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
        public async Task<IActionResult> RemoveAlbumImage(string id, string sId)
        {
            var album = await _dbContext.Albums
                .Include(q => q.Image)
                .Include(q => q.Artist)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (album is null)
                return BadRequest();
            if (album.Artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();

            if (album.Image?.Id == sId)
            {
                _dbContext.AlbumImages.Remove(album.Image);
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [Authorize(Policy = "write:admin")]
        [HttpPost("{id}/track/{sId}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadTrack(string id, string sId, IFormFile file)
        {
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest();
            if (file is null || file.Length < 0)
                return BadRequest();
            if (!file.ContentType.StartsWith("audio/"))
                return StatusCode(415);

            var track = await _dbContext.Tracks
                .Include(q => q.Album)
                    .ThenInclude(q => q.Artist)
                .Where(q => q.Id == sId)
                .FirstOrDefaultAsync();

            if (track is null)
                return NotFound();
            if (track.Album.Artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            if (track.Album.Id != id)
                return BadRequest();

            var trackFile = track.TrackFile ?? new TrackFile();
            track.TrackFile = trackFile;
            if (String.IsNullOrEmpty(trackFile.Id))
                await _dbContext.TrackFiles.AddAsync(trackFile);


            var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            switch (ext)
            {
                case ".mp3":
                    await using (var stream = file.OpenReadStream())
                    {
                        track.Duration = new Mp3FileReader(stream).TotalTime;
                    }
                    break;
                case ".wav":
                    await using (var stream = file.OpenReadStream())
                    {
                        track.Duration = new WaveFileReader(stream).TotalTime;
                    }
                    break;
                default:
                    return BadRequest();
            }

            var container = _blobService.GetBlobContainerClient(_config.Value.ContainerTrack);
            await container.CreateIfNotExistsAsync(PublicAccessType.None);
            var blob = container.GetBlobClient(trackFile.Id);
            await blob.DeleteIfExistsAsync();
            await using (var stream = file.OpenReadStream())
            {
                try
                {
                    await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                }
                catch (Exception)
                {
                    return Problem();
                }
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy = "remove:admin")]
        [HttpDelete("{id}/track/{sId}")]
        public async Task<IActionResult> RemoveTrackFile(string id, string sId)
        {
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest();
            var track = await _dbContext.Tracks
                .Include(q => q.Album)
                    .ThenInclude(q => q.Artist)
                .Include(q => q.TrackFile)
                .Where(q => q.Id == sId && q.Album.Id == id)
                .FirstOrDefaultAsync();
            if (track is null)
                return NotFound();
            if (track.Album.Artist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            if (track.TrackFile is null)
                return Ok();

            _dbContext.Remove(track.TrackFile);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
