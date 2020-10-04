using Microsoft.EntityFrameworkCore;
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
        public DbSet<Image> Images { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
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

        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
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
    }
}
