using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Dtos
{
    public class PlaylistDto
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public bool IsPublic { get; set; }
        public bool IsMy { get; set; } = false;
        public int NumberOf { get; set; }
        public ImageDto Image { get; set; }
        public List<PlaylistElementDto> PlaylistElements { get; set; }

        public string Type => "Playlist";
    }
}
