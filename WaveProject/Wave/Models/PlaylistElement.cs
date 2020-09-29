using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wave.Interfaces;

namespace Wave.Models
{
    public class PlaylistElement : IAuditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string PlayListId { get; set; }
        public Playlist Playlist { get; set; }

        public string TrackId { get; set; }
        public Track Track { get; set; }

        public int NumberOf { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }
    }
}
