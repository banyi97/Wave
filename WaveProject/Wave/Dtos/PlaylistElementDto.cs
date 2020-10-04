using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Dtos
{
    public class PlaylistElementDto
    {
        public string Id { get; set; }
        public string PlayListId { get; set; }

        public int NumberOf { get; set; }

        public DateTime CreateDate { get; set; }
        public PlaylistTrackDto Track { get; set; }
    }
}
