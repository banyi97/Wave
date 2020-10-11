using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Models
{
    public class ArtistImage : Image
    {
        public string ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}
