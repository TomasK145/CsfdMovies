using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csfd
{
    public class CsfdApi
    {
        private readonly MovieParser _movieParser = new MovieParser();
        private readonly SearchParser _searchParser = new SearchParser();

        public Movie GetMovie(string url)
        {
            return _movieParser.GetMovie(url);
        }

        public Movie SearchMovie(string query)
        {
            return _movieParser.SearchAndGetMovie(query);
        }

        public void GetSearch(HashSet<string> movieList, string url)
        {
            _searchParser.GetSearchPage(movieList, url);
        }
    }
}
