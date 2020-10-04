using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Dtos
{
    public class CreateTrackDto
    {
        public string Title { get; set; }
        public string AlbumId { get; set; }

        public bool IsExplicit { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsPlayable { get; set; }
        public int DiscNumber { get; set; }
        public int NumberOf { get; set; }
    }
}
