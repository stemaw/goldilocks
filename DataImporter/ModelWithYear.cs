namespace DataImporter
{
    public class ModelWithYear
    {
        public string ModelName { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}{2})", ModelName, YearFrom.ToString().Substring(2, 2), GetEnd());
        }

        private string GetEnd()
        {
            if (YearTo == 0) return " on";

            return "-" + YearTo.ToString().Substring(2, 2);
        }
    }

    
}