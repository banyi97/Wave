using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wave.Interfaces;

namespace Wave.Services
{
    public static class RenumberExtension
    {
        public static void Renumber(this IEnumerable<INumberOf<int>> list)
        {
            try
            {
                var i = 0;
                foreach (var item in list)
                    item.NumberOf = i++;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
