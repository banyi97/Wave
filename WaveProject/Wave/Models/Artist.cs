using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wave.Interfaces;

namespace Wave.Models
{
    public class Artist : IAuditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<Album> Albums { get; set; } = new List<Album>();
        public ArtistImage Image { get; set; }

        public string ApplicationUserId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }
    }
}
