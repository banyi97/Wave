using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wave.Interfaces;

namespace Wave.Models
{
    public class Album : IAuditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string ArtistId { get; set; }
        public Artist Artist { get; set; }

        public string Label { get; set; }
        public DateTime ReleaseDate { get; set; }
        public ReleaseDatePrecision ReleaseDatePrecision { get; set; }
        public AlbumType AlbumType { get; set; }

        public List<Track> Tracks { get; set; } = new List<Track>();
        public Image Image { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }
    }
}
