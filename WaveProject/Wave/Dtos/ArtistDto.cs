using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Dtos
{
    public class ArtistDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ImageDto Image { get; set; }
    }
}
