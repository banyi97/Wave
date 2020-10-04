using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Dtos
{
    public class PlaylistTrackDto
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public string AlbumId { get; set; }
        public string AlbumLabel { get; set; }

        public string ArtistId { get; set; }
        public string ArtistName { get; set; }

        public string Uri { get; set; }
        public string AlbumImageUri { get; set; }

        public bool IsExplicit { get; set; }
        public bool IsPlayable { get; set; }
        public double Duration { get; set; }
    }
}
