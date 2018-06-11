using System.Linq;

namespace Csfd
{
    public class Movie
    {
        public string Url { get; set; }
        public string TitleOrigin { get; set; }
        public string TitleSk { get; set; }
        public string Year { get; set; }
        public int Rating { get; set; }
        public string OriginCountry { get; set; }

        public Movie(string url, string titles, string titleSk, string year, int rating, string originCountry)
        {
            Url = url;
            TitleSk = new string(titleSk.Where(c => !char.IsControl(c)).ToArray()).Replace(",", " ");
            Year = year;
            Rating = rating;
            OriginCountry = originCountry;

            var titlesSplit = new string(titles.Where(c => !char.IsControl(c)).ToArray()).Replace(",", " ").Split('/');
            TitleOrigin = (titlesSplit.Length == 2) ? titlesSplit[1] : titlesSplit[0];
        }

        public override string ToString()
        {            
            return $"{TitleOrigin},{TitleSk},{Year},{OriginCountry},{Url},{Rating}";
        }
    }
}
