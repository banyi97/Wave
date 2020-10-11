using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Models
{
    public class AlbumImage : Image
    {
        public string AlbumId { get; set; }
        public Album Album { get; set; }
    }
}
