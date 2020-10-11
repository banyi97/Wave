using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wave.Database;
using Wave.Dtos;
using Wave.Models;
using Wave.Services;

namespace Wave.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PlaylistController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly BlobServiceClient _blobService;
        private readonly IOptions<AzureBlobConfig> _config;

        public PlaylistController(
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
            var playlists = await _dbContext.Playlists
                    .Where(q => q.ApplicationUserId == this.User.Identity.Name)
                    .OrderBy(q => q.NumberOf)
                    .ThenByDescending(q => q.LatestUpdate)
                    .ToListAsync();
            var playlist = playlists.SingleOrDefault(q => q.Id == id);
            if (playlist is null)
                return NotFound();
            if (playlist.NumberOf != from || !(to >= 0 && to < playlists.Count))
                return BadRequest();
            if (playlist.NumberOf == to)
                return Ok();
            try
            {
                var element = playlists[playlist.NumberOf];
                playlists.RemoveAt(playlist.NumberOf);
                playlists.Insert(to, element);
                playlists.Renumber();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            await _dbContext.SaveChangesAsync();
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
                    ret.IsMy = true; // drag and drop
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

        [HttpPatch("{id}/publis")]
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
            var playlists = await _dbContext.Playlists
                    .Where(q => q.ApplicationUserId == this.User.Identity.Name)
                    .OrderBy(q => q.NumberOf)
                    .ThenByDescending(q => q.LatestUpdate)
                    .ToListAsync();

            var playlist = playlists.SingleOrDefault(q => q.Id == id);
            if (playlist is null)
                return Ok();
            if (playlist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();

            playlists.Remove(playlist);
            _dbContext.Playlists.Remove(playlist);
            playlists.Renumber();

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/images")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPlaylistImage([FromRoute] string id, IFormFile file)
        {
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest();
            if (file is null || file.Length < 0)
                return BadRequest("file null");
            if (!file.ContentType.StartsWith("image/"))
                return StatusCode(415);

            var playlist = await _dbContext.Playlists
                .Include(q => q.Image)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (playlist.Image != null)
                _dbContext.PlaylistImages.Remove(playlist.Image);

            var img = new PlaylistImage();
            playlist.Image = img;
            await _dbContext.PlaylistImages.AddAsync(img);

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

        [HttpDelete("{id}/images/{sId}")]
        public async Task<IActionResult> RemovePlaylistImage(string id, string sId)
        {
            var playlist = await _dbContext.Playlists
                .Include(q => q.Image)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (playlist is null)
                return BadRequest();

            if (playlist.Image?.Id == sId)
            {
                _dbContext.PlaylistImages.Remove(playlist.Image);
                await _dbContext.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost("{id}/{sId}")]
        public async Task<IActionResult> AddToPlaylist(string id, string sId)
        {
            var playlist = await _dbContext.Playlists.FindAsync(id);
            if (playlist is null)
                return NotFound();
            if (playlist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            var song = await _dbContext.Tracks.FindAsync(sId);
            if (song is null)
                return NotFound();

            var elements = await _dbContext.PlaylistElements
                    .Include(q => q.Playlist)
                    .Where(q => q.PlayListId == id)
                    .OrderBy(q => q.NumberOf)
                    .ThenByDescending(q => q.LatestUpdate)
                    .ToListAsync();

            var element = new PlaylistElement
            {
                PlayListId = id,
                TrackId = sId,
                NumberOf = elements.LastOrDefault() == null ? 0 : elements.Last().NumberOf + 1,
            };
            await _dbContext.PlaylistElements.AddAsync(element);
            await _dbContext.SaveChangesAsync();
            return Ok(_mapper.Map<PlaylistElement, PlaylistElementDto>(element));
        }

        [HttpPut("{id}/{sId}")]
        public async Task<IActionResult> ReOrderPlaylist(string id, string sId, [FromQuery] int next)
        {
            var elements = await _dbContext.PlaylistElements
                    .Include(q => q.Playlist)
                    .Where(q => q.PlayListId == id)
                    .OrderBy(q => q.NumberOf)
                    .ThenByDescending(q => q.LatestUpdate)
                    .ToListAsync();
            var element = elements.SingleOrDefault(q => q.Id == sId);
            if (element is null)
                return Ok();
            if (element.Playlist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            try
            {
                var plElement = elements[element.NumberOf];
                elements.RemoveAt(element.NumberOf);
                elements.Insert(next, plElement);
                elements.Renumber();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}/{sId}")]
        public async Task<IActionResult> RemoveFromPlaylist(string id, string sId)
        {
            var element = await _dbContext.PlaylistElements.Include(q => q.Playlist).Where(q => q.Id == sId).SingleOrDefaultAsync();
            if (element is null)
                return Ok();
            if (element.Playlist.ApplicationUserId != this.User.Identity.Name)
                return Forbid();
            if (element.Playlist.Id != id)
                return BadRequest();

            var elements = await _dbContext.PlaylistElements
                .Where(q => q.PlayListId == element.PlayListId)
                .OrderBy(q => q.NumberOf)
                .ThenByDescending(q => q.LatestUpdate)
                .ToListAsync();

            elements.Remove(element);
            _dbContext.PlaylistElements.Remove(element);
            elements.Renumber();

            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
