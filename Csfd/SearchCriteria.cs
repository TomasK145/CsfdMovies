using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csfd
{
    public class SearchCriteria
    {
        public Country Countries { get; }
        public Genre Genres { get; }
        public List<YearRange> YearRanges { get; private set; }



    }
}
