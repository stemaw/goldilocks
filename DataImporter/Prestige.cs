using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarChooser.Data;
using CarChooser.Domain;

namespace DataImporter
{
    public class Prestige
    {
        public void ImportManufacturerScores()
        {
            var sources = GetScores();

            var repo = new CarRepository();

            foreach (var manufacturer in sources)
            {
                var updated = repo.UpdatePrestiage(manufacturer.Key, manufacturer.Value);

                if (!updated)
                {
                    Console.WriteLine("Failed to update {0}", manufacturer.Key);
                }
            }
        }

        public Dictionary<string, int> GetScores()
        {
            var source = new Dictionary<string, int>();

            source.Add("Honda", 65);
            source.Add("Daihatsu", 20);
            source.Add("Suzuki", 30);
            source.Add("Toyota", 50);
            source.Add("Chevrolet", 45);
            source.Add("Mazda", 52);
            source.Add("Ford", 49);
            source.Add("Lexus", 70);
            source.Add("Skoda", 54);
            source.Add("Hyundai", 47);
            source.Add("Subaru", 62);
            source.Add("Nissan", 50);
            source.Add("Daewoo", 34);
            source.Add("Peugeot", 41);
            source.Add("Fiat", 51);
            source.Add("Citroen", 38);
            source.Add("Mitsubishi", 51);
            source.Add("Smart", 57);
            source.Add("Kia", 48);
            source.Add("Vauxhall", 34);
            source.Add("Seat", 57);
            source.Add("Renault", 42);
            source.Add("Mini", 72);
            source.Add("Volkswagen", 68);
            source.Add("Volvo", 64);
            source.Add("Saab", 67);
            source.Add("BMW", 84);
            source.Add("MG", 20);
            source.Add("Jaguar", 83);
            source.Add("SsangYong", 10);
            source.Add("Mercedes-Benz", 86);
            source.Add("Chrysler", 37);
            source.Add("Audi", 83);
            source.Add("Jeep", 55);
            source.Add("Alfa Romeo", 72);
            source.Add("Land Rover", 89);
            source.Add("Porsche", 90);
            source.Add("Bentley", 100);
            source.Add("Tesla", 95);

            return source;
        }
    }

    public class ManufacturerComparer : IEqualityComparer<Car>
    {
        public bool Equals(Car x, Car y)
        {
            return x.Manufacturer.Name == y.Manufacturer.Name;
        }

        public int GetHashCode(Car obj)
        {
            return obj.Manufacturer.Name.GetHashCode();
        }
    }
}
