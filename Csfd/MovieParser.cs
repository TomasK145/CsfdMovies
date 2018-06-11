using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace Csfd
{
    internal class MovieParser
    {
        private readonly HtmlLoader _htmlLoader = new HtmlLoader();

        /// <summary>
        ///     Loads movie from Csfd.cz URL and returns movie object
        /// </summary>
        /// <param name="url">URL of movie at CSFD.cz</param>
        /// <returns>MovieParser class</returns>
        internal Movie GetMovie(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return null;
            }
            var document = _htmlLoader.GetDocumentByUrl(url);

            var titles = GetTitles(document);
            string titleSk = GetTitleSk(document);
            var year = GetYear(document);
            int rating = GetRating(document);
            var originCountry = GetOriginCountry(document);

            return new Movie(url, titles, titleSk, year, rating, originCountry);
        }

        /// <summary>
        ///     Search for movie by query and return first result as movie object
        /// </summary>
        /// <param name="query">Query to search for</param>
        /// <returns>MovieParser object</returns>
        internal Movie SearchAndGetMovie(string query)
        {
            var movie = GetMovie(SearchMovie(query));
            return movie;
        }

        /// <summary>
        ///     Performs search on CSFD cz for given query
        /// </summary>
        /// <param name="query">Query to search for</param>
        /// <returns>URL of first result</returns>
        private string SearchMovie(string query)
        {
            var document = _htmlLoader.GetDocumentByUrl("http://www.csfd.cz/hledat/?q=" + query);

            var node = document.DocumentNode.SelectSingleNode("//*[@id='search-films']/div[1]/ul[1]/li[1]/div/h3/a");

            // Some CSFD searches redirect directly to MovieURL, get current url from 'comments' link
            if (node == null)
            {
                node = document.DocumentNode.SelectSingleNode("//*[@id='main']/div[4]/div[1]/ul/li[1]/a");
                if (node == null)
                {
                    Console.WriteLine($"Failed to find movie with query '{query}'");
                    return string.Empty;
                }
            }
            var movieUrl = node.Attributes["href"].Value;

            return "http://www.csfd.cz" + movieUrl;
        }

        /// <summary>
        ///     Returns CZ title / International title
        /// </summary>
        /// >
        /// <param name="doc">HtmlDocument loaded from csfd.cz movie</param>
        /// <returns>String in format: "Czech title / International title"</returns>
        private string GetTitles(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectNodes("//title");
            var titlesAndYear = node[0].InnerText.Trim();
            // Remove "| CSFD.cz"
            titlesAndYear = titlesAndYear.Substring(0, titlesAndYear.LastIndexOf("|", StringComparison.Ordinal)).Trim();

            // Check if year is present and remove it from title
            var indexOfLeftBrace = titlesAndYear.LastIndexOf("(", StringComparison.Ordinal);
            return indexOfLeftBrace > 0 ? titlesAndYear.Substring(0, indexOfLeftBrace).Trim() : titlesAndYear;
        }

        private string GetTitleSk(HtmlDocument doc)
        {
            string titleSk = string.Empty;
            //var node = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[2]/div[1]/div[1]/div[1]/div[2]/div[1]"); //OK
            var node = doc.DocumentNode.SelectNodes("//div[@class='header']//h1");
            if (node != null)
            {
                titleSk = node[0].InnerText.Trim();
            }

            return titleSk;
        }

        /// <summary>
        ///     Returns Year the movie was released
        /// </summary>
        /// <param name="doc">HtmlDocument loaded from csfd.cz movie</param>
        /// <returns>String year</returns>
        private string GetYear(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectNodes("//title");
            var titlesAndYear = node[0].InnerText.Trim();
            var searchYear = new Regex(@"\(\d{4}\)");
            var result = searchYear.Match(titlesAndYear);
            var year = result.Value.Replace("(", string.Empty);
            year = year.Replace(")", string.Empty);
            return year;
        }

        /// <summary>
        ///     Get movie rating %
        /// </summary>
        /// <param name="document">HtmlDocument</param>
        /// <returns>Rating</returns>
        private int GetRating(HtmlDocument document)
        {
            int rating;

            var node = document.DocumentNode.SelectSingleNode("//h2[@class='average']");

            try
            {
                rating = int.Parse(Regex.Match(node.InnerText, @"\d*").Value);
            }
            catch
            {
                rating = -1;
            }

            return rating;
        }

        /// <summary>
        ///     Get movie origin country
        /// </summary>
        /// <param name="document">HtmlDocument</param>
        /// <returns>originCountry</returns>
        private string GetOriginCountry(HtmlDocument document)
        {
            string originCountry = string.Empty;

            //var node = document.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[2]/div[1]/div[1]/div[1]/div[2]/p[2]").FirstOrDefault(); //OK
            var node = document.DocumentNode.SelectSingleNode("//p[@class='origin']");
            originCountry = node?.InnerText.Split(',')[0];

            return originCountry;
        }
    }
}
