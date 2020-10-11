using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wave.Interfaces;

namespace Wave.Models
{
    public abstract class Image : IAuditable, ICascadeRemovable<string>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LatestUpdate { get; set; }

        public virtual string CascadeId => this.Id;
    }
}
