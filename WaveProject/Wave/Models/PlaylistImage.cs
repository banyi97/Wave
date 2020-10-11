using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Models
{
    public class PlaylistImage : Image
    {
        public string PlaylistId { get; set; }
        public Playlist Playlist { get; set; }
    }
}
