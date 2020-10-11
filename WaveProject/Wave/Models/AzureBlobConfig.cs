using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Models
{
    public class AzureBlobConfig
    {
        public string BlobUri { get; set; }
        public string ConnectionString { get; set; }
        public string ContainerTrack { get; set; }
        public string ContainerImg { get; set; }
    }
}
