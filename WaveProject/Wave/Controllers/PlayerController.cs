using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wave.Database;
using Wave.Models;

namespace Wave.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly BlobServiceClient _blobService;
        private readonly IOptions<AzureBlobConfig> _config;

        public PlayerController(
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

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStream(string id)
        {
            var track = await _dbContext.Tracks
                .Where(q => q.Id == id)
                .Include(q => q.TrackFile)
                .FirstOrDefaultAsync();
            if (track is null)
                return NotFound();

            var container = _blobService.GetBlobContainerClient(_config.Value.ContainerTrack);
            var blob = container.GetBlobClient(track.TrackFile.Id);
            if (!await blob.ExistsAsync())
                return NotFound();
            await using var stream = new MemoryStream();
            using var resp = await blob.DownloadToAsync(stream);
            return File(stream.ToArray(), resp.Headers.ContentType, true);
        }

        [Authorize]
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
            catch (Exception)
            {
                return Ok();
            }
        }
    }
}
