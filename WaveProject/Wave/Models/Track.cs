using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Models
{
    public class Track
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string AlbumId { get; set; }
        public Album Album { get; set; }

        public string Title { get; set; }
        public int Plays { get; set; }
        public bool IsExplicit { get; set; }
        public TimeSpan Duration { get; set; }
        public int DiscNumber { get; set; }
        public int NumberOf { get; set; }

        public List<PlaylistElement> TrackOfPlaylistElements { get; set; } = new List<PlaylistElement>();
        public TrackFile TrackFile { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }

        [NotMapped]
        public virtual bool IsPlayable
        {
            get
            {
                return true;
            }
        }
    }
}

