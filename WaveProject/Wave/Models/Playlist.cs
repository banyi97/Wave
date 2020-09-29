using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wave.Interfaces;

namespace Wave.Models
{
    public class Playlist : IAuditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string ApplicationUserId { get; set; }

        public string Title { get; set; }
        public bool IsPublic { get; set; }
        public int NumberOf { get; set; }

        public List<PlaylistElement> PlaylistElements { get; set; } = new List<PlaylistElement>();
        public Image Image { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }
    }
}
