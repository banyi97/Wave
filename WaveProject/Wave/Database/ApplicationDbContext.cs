using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wave.Interfaces;
using Wave.Models;

namespace Wave.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<TrackFile> TrackFiles { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistElement> PlaylistElements { get; set; }

        public DbSet<AlbumImage> AlbumImages { get; set; }
        public DbSet<ArtistImage> ArtistImages { get; set; }
        public DbSet<PlaylistImage> PlaylistImages { get; set; }

        private readonly BlobServiceClient _blobService;
        private readonly IOptions<AzureBlobConfig> _config;

        public ApplicationDbContext(
            DbContextOptions options,
            BlobServiceClient blobService,
            IOptions<AzureBlobConfig> config) : base(options)
        {
            _blobService = blobService ?? throw new NullReferenceException();
            _config = config ?? throw new NullReferenceException();
        }  

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Track>()
                .Property(q => q.DiscNumber).HasDefaultValue(0);
            builder.Entity<Track>()
                .Property(q => q.Plays).HasDefaultValue(0);
            builder.Entity<Track>()
                .Property(q => q.IsExplicit).HasDefaultValue(false);

            builder.Entity<Playlist>()
                .Property(q => q.IsPublic).HasDefaultValue(false);

            builder.Entity<Album>()
                .HasOne(q => q.Artist)
                .WithMany(w => w.Albums)
                .HasForeignKey(e => e.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Track>()
                .HasOne(q => q.Album)
                .WithMany(w => w.Tracks)
                .HasForeignKey(e => e.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TrackFile>()
                .HasOne(q => q.Track)
                .WithOne(q => q.TrackFile)
                .HasForeignKey<TrackFile>(q => q.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PlaylistElement>()
                .HasOne(pe => pe.Playlist)
                .WithMany(pl => pl.PlaylistElements)
                .HasForeignKey(pe => pe.PlayListId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PlaylistElement>()
                .HasOne(q => q.Track)
                .WithMany(s => s.TrackOfPlaylistElements)
                .HasForeignKey(pe => pe.TrackId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ArtistImage>()
                .HasOne(q => q.Artist)
                .WithOne(q => q.Image)
                .HasForeignKey<ArtistImage>(q => q.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AlbumImage>()
                .HasOne(q => q.Album)
                .WithOne(q => q.Image)
                .HasForeignKey<AlbumImage>(q => q.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PlaylistImage>()
                .HasOne(q => q.Playlist)
                .WithOne(q => q.Image)
                .HasForeignKey<PlaylistImage>(q => q.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            await DeletedEntityAsync();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateAuditEntities()
        {
            var modifiedEntries = base.ChangeTracker.Entries()
                .Where(q => q.Entity is IAuditable && (q.State == EntityState.Added || q.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                var entity = (IAuditable)entry.Entity;
                DateTime now = DateTime.UtcNow;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                }
                else
                {
                    base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }
                entity.LatestUpdate = now;
            }
        }

        private async Task DeletedEntityAsync()
        {
            var allRemovedEntries = base.ChangeTracker.Entries()
                .Where(q => q.Entity is ICascadeRemovable<string> && q.State == EntityState.Deleted);
            
            var imgContainer = _blobService.GetBlobContainerClient(_config.Value.ContainerImg);
            var trackContainer = _blobService.GetBlobContainerClient(_config.Value.ContainerTrack);

            foreach (var item in allRemovedEntries)
            {
                var entity = (ICascadeRemovable<string>)item.Entity;
                if (item.Entity is Image)
                {
                    await imgContainer.GetBlobClient(entity.CascadeId).DeleteIfExistsAsync();
                }
                else if (item.Entity is TrackFile)
                {
                    await trackContainer.GetBlobClient(entity.CascadeId).DeleteIfExistsAsync();
                }
            }
        }

    }
}
