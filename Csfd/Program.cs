using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Csfd
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<Movie> movies = new List<Movie>();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                //najskor inicializuj list datami z DB
                HashSet<string> movieNames = GetMovieNames();
                sw.Stop();
                Console.WriteLine("Ziskanie zoznamu filmov - trvanie: " + sw.ElapsedMilliseconds + " ms");

                sw.Restart();
                foreach (string movieName in movieNames)
                {                    
                    Movie movie = GetMovie(movieName);
                    if (movie == null)
                    {
                        continue;
                    }
                    sb.AppendLine(movie.ToString());
                    //movies.Add(movie); //TODO: treba pridavat filmy aj do listu filmov??? vyuzitie???

                    sw.Stop();
                    Console.WriteLine("Ziskanie filmu " + movieName + " - duration: " + sw.ElapsedMilliseconds + " ms");
                    sw.Restart();
                }
                sw.Stop();
                Console.WriteLine("Ziska info o jednotlivych filmoch - trvanie: " + sw.ElapsedMilliseconds + " ms");

                Console.WriteLine("GetMovie done");
                ExportMoviesToCsv(sb.ToString());
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static Movie GetMovie(string movieName)
        {
            CsfdApi api = new CsfdApi();
            Movie movie = api.SearchMovie(movieName);
            return movie;
        }

        private static HashSet<string> GetMovieNames()
        {
            HashSet<string> movieList = new HashSet<string>();
            List<YearRange> yearRanges = GetYearRangeList();
            foreach (Country country in Enum.GetValues(typeof(Country)))
            {
                foreach (Genre genre in Enum.GetValues(typeof(Genre)))
                {
                    foreach (YearRange yearRange in yearRanges)
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        HashSet<string> movieListCurrent = GetMovieNamesBySearchCriteria(country, genre, yearRange);
                        movieList.UnionWith(movieListCurrent);

                        sw.Stop();
                        Console.WriteLine("Country: " + country + " - Genre: " + genre + " - YearRange: " + yearRange.ToString() + " - MoviesCount: " + movieListCurrent.Count + " - duration: " + sw.ElapsedMilliseconds + " ms");
                        sw.Restart();
                    }
                }
            }
            return movieList;
        }        

        private static HashSet<string> GetMovieNamesBySearchCriteria(Country country, Genre zaner, YearRange yearRange)
        {          
            CsfdApi api = new CsfdApi();
            HashSet<string> movieList = new HashSet<string>();

            string originCountry = ((int)country).ToString();
            string genre = ((int)zaner).ToString();
            string searchUrl = $"https://www.csfd.cz/zebricky/specificky-vyber/?type=0&origin={originCountry}&genre={genre}&year_from={yearRange.YearFrom}&year_to={yearRange.YearTo}&actor=&director=&ok=Zobrazit&_form_=charts&show=complete";
            api.GetSearch(movieList, searchUrl);

            //todo: uloz zoznam filmov ziskanych z danej url
            var movieInfo = searchUrl + " --> " + String.Join(";", movieList);
            using (StreamWriter file = new StreamWriter(@"C:\Users\Public\MovieInfo.txt", true))
            {
                file.WriteLine(movieInfo);
            }

            return movieList;
        }

        private static List<YearRange> GetYearRangeList()
        {
            List<YearRange> years = new List<YearRange>()
            {
                new YearRange("","1920"),
                new YearRange("1920","1925"),
                new YearRange("1925","1930"),
                new YearRange("1935","1940"),
                new YearRange("1940","1945"),
                new YearRange("1945","1950"),
                new YearRange("1950","1955"),
                new YearRange("1955","1960"),
                new YearRange("1960","1965"),
                new YearRange("1965","1970"),
                new YearRange("1970","1975"),
                new YearRange("1975","1980"),
                new YearRange("1980","1985"),
                new YearRange("1985","1990"),
                new YearRange("1990","1995"),
                new YearRange("1995","2000"),
                new YearRange("2000","2005"),
                new YearRange("2005","2010"),
                new YearRange("2010","2015"),
                new YearRange("2015",""),
            };            

            return years;
        }


        private static void ExportMoviesToCsv(string text)
        {
            StringBuilder sb = new StringBuilder();
            const string FilePath = @"C:\Users\Public\Movies.csv";

            sb.AppendLine("Nazov original,Nazov SK,Rok produkcie,Stat produkcie,URL,Hodnotenie");
            sb.AppendLine(text);

            using (StreamWriter file = new StreamWriter(FilePath, true))
            {
                file.WriteLine(sb.ToString());
            }
        }
    }
}
