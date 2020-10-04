using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wave.Models;

namespace Wave.Dtos
{
    public class CreateAlbumDto
    {
        public string Label { get; set; }

        public string ArtistId { get; set; }

        public DateTime ReleaseDate { get; set; }
        public ReleaseDatePrecision ReleaseDatePrecision { get; set; }
        public AlbumType AlbumType { get; set; }

        public DateTime AvailableFromUtfTime { get; set; }

        public List<CreateTrackDto> Tracks { get; set; }
    }
}
