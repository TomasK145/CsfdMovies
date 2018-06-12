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
                sb.AppendLine("Nazov original,Nazov SK,Rok produkcie,Stat produkcie,URL,Hodnotenie");
                List<Movie> movies = new List<Movie>();

                Stopwatch sw = new Stopwatch();
                sw.Start();
                HashSet<string> movieNames = TestSearch();
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
                }
                sw.Stop();
                Console.WriteLine("Ziska info o jednotlivych filmoch - trvanie: " + sw.ElapsedMilliseconds + " ms");

                Console.WriteLine("GetMovie done");
                WriteFile(sb.ToString());
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

        private static HashSet<string> TestSearch()
        {
            HashSet<string> movieList = new HashSet<string>();
            CsfdApi api = new CsfdApi();
            Stopwatch sw = new Stopwatch();

            ////for (int i = 1; i<= 250; i++)
            ////TODO: ake krajiny??? inicializuj pole krajin
            //for (int i = 179; i <= 179; i++) //172 - USA, 44 - Francuzsko, 61 - Taliansko, 179 - VB
            //{
            //    //TODO: inicializuj pole zanrov
            //    //for (int j = 1; j <= 45; j++) //1 - akcne
            //    for (int j = 1; j <= 1; j++) //1 - akcne
            //    {
            //        string origin = Convert.ToString(i);
            //        string genre = Convert.ToString(j);

            //        //string searchUrl = $"https://www.csfd.cz/zebricky/specificky-vyber/?type=0&origin={origin}&genre={genre}&year_from=&year_to=2000&actor=&director=&ok=Zobrazit&_form_=charts&show=complete";
            //        //api.GetSearch(movieList, searchUrl);

            //        //searchUrl = $"https://www.csfd.cz/zebricky/specificky-vyber/?type=0&origin={origin}&genre={genre}&year_from=2001&year_to=2005&actor=&director=&ok=Zobrazit&_form_=charts&show=complete";
            //        //api.GetSearch(movieList, searchUrl);

            //        //searchUrl = $"https://www.csfd.cz/zebricky/specificky-vyber/?type=0&origin={origin}&genre={genre}&year_from=2006&year_to=2010&actor=&director=&ok=Zobrazit&_form_=charts&show=complete";
            //        //api.GetSearch(movieList, searchUrl);

            //        string searchUrl = $"https://www.csfd.cz/zebricky/specificky-vyber/?type=0&origin={origin}&genre={genre}&year_from=2011&year_to=2017&actor=&director=&ok=Zobrazit&_form_=charts&show=complete";
            //        api.GetSearch(movieList, searchUrl);
            //    }                
            //}

            sw.Start();
            foreach (Country country in Enum.GetValues(typeof(Country)))
            {
                foreach (Genre zaner in Enum.GetValues(typeof(Genre)))
                {
                    foreach (YearRange year in GetYearRangeList())
                    {
                        string origin = ((int)country).ToString();
                        string genre = ((int)zaner).ToString();

                        string searchUrl = $"https://www.csfd.cz/zebricky/specificky-vyber/?type=0&origin={origin}&genre={genre}&year_from={year.YearFrom}&year_to={year.YearTo}&actor=&director=&ok=Zobrazit&_form_=charts&show=complete";
                        api.GetSearch(movieList, searchUrl);

                        sw.Stop();
                        Console.WriteLine("Country: " + origin + " - Genre: " + genre + " - YearRange: " + year.ToString() + " - MoviesCount: " + movieList.Count + " - duration: " + sw.ElapsedMilliseconds + " ms");
                        sw.Restart();
                    }
                    
                }
            }

            //TODO: nejakym sposobom ukladat vysledky persistentne

            


            Console.WriteLine("TestSearch done");
            return movieList;
        }

        private static List<YearRange> GetYearRangeList()
        {
            List<YearRange> years = new List<YearRange>()
            {
                new YearRange("","1980"),
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


        private static void WriteFile(string text)
        {
            using (StreamWriter file = new StreamWriter(@"C:\Users\Public\Movies.csv", true))
            {
                file.WriteLine(text);
            }
        }
    }
}
