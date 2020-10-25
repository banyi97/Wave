using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Dtos
{
    public class ReorderDto
    {
        public string Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}
