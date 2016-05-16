namespace CarChooser.Domain
{
    public class Manufacturer
    {
        private string _name;

        public string Name
        {
            get {
                return _name.StartsWith("Citr") ? "Citroen" : _name;
            }
            set { _name = value; }
        }

        public int Score { get; set; }

        public int ReliabilityIndex { get; set; }

        public int PrestigeIndex { get; set; }
    }
}