namespace Csfd
{
    public class YearRange
    {
        public string YearFrom { get; set; }
        public string YearTo { get; set; }

        public YearRange(string yearFrom, string yearTo)
        {
            YearFrom = yearFrom;
            YearTo = yearTo;
        }

        public override string ToString()
        {
            return $"YearFrom: {YearFrom} - YearTo: {YearTo}";
        }
    }
}
