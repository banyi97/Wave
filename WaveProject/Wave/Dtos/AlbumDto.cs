using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wave.Models;

namespace Wave.Dtos
{
    public class AlbumDto
    {
        public string Id { get; set; }
        public string Label { get; set; }

        public string ArtistId { get; set; }
        public string ArtistName { get; set; }

        public DateTime ReleaseDate { get; set; }
        public ReleaseDatePrecision ReleaseDatePrecision { get; set; }
        public AlbumType AlbumType { get; set; }

        public List<TrackDto> Tracks { get; set; }
        public ImageDto Image { get; set; }

        public string Type => "Album";
    }
}
