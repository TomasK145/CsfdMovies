using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csfd
{
    internal class SearchParser
    {
        private readonly HtmlLoader _htmlLoader = new HtmlLoader();

        internal void GetSearchPage(HashSet<string> movieList, string url)
        {
            var document = _htmlLoader.GetDocumentByUrl(url);
            GetTitles(movieList, document);
        }

        private void GetTitles(HashSet<string> movieList, HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//*[@class='film']");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    movieList.Add(node.InnerText);
                }
            }
        }
    }
}
