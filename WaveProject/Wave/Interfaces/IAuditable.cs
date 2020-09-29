using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Interfaces
{
    public class IAuditable
    {
        DateTime CreatedDate { get; set; }
        DateTime LatestUpdate { get; set; }
    }
}
