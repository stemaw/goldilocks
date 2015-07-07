using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarChooser.Data;
using CarChooser.Domain;

namespace DataImporter
{
    public class ReliabilityIndex
    {
        public void FindManufacturersWithNoRating()
        {
            var repo = new CarRepository();

            var allCars = repo.AllCars().Where(c => c.Manufacturer.ReliabilityIndex == 0);


            var allManufacturers = from car in allCars
                                   group car by car.Manufacturer.Name
                                       into grp
                                       select grp.Key;

            //var source = GetScores();
            //var missing = allManufacturers.Intersect(source.Select(s => s.Key));

            foreach (var miss in allManufacturers)
            {
                Console.WriteLine(miss);
            }
        }

        public void ImportManufacturerScores()
        {
            var sources = GetNewScores();

            var repo = new CarRepository();

            var all = repo.AllCars().Distinct(new ManufacturerComparer());

            //foreach (var m in all)
            //{
            //    Console.WriteLine(m.Manufacturer.Name);
            //}

            //Console.ReadLine();
            foreach (var manufacturer in sources)
            {
                var updated = repo.UpdateManufacturerScore(manufacturer.Key, manufacturer.Value);

                if (!updated)
                {
                    Console.WriteLine("Failed to update {0}", manufacturer.Key);
                }
            }
        }

        public Dictionary<string, int> GetNewScores()
        {
            var source = new Dictionary<string, int>();

            source.Add("Citro&#235;n", 106);
            source.Add("SEAT", 126);
            source.Add("MINI", 137);
            source.Add("Abarth", 98);

            return source;
        }

        public Dictionary<string, int> GetScores()
        {
            var source = new Dictionary<string, int>();

            source.Add("Honda", 42);
            source.Add("Daihatsu", 44);
            source.Add("Suzuki", 49);
            source.Add("Toyota", 67);
            source.Add("Chevrolet", 82);
            source.Add("Mazda", 85);
            source.Add("Ford", 86);
            source.Add("Lexus", 89);
            source.Add("Skoda", 91);
            source.Add("Hyundai", 92);
            source.Add("Subaru", 93);
            source.Add("Nissan", 95);
            source.Add("Daewoo", 97);
            source.Add("Peugeot", 98);
            source.Add("Fiat", 98);
            source.Add("Citroen", 106);
            source.Add("Mitsubishi", 110);
            source.Add("Smart", 111);
            source.Add("Kia", 116);
            source.Add("Vauxhall", 123);
            source.Add("Seat", 126);
            source.Add("Renault", 130);
            source.Add("Mini", 137);
            source.Add("Volkswagen", 138);
            source.Add("Rover", 150);
            source.Add("Volvo", 152);
            source.Add("Saab", 159);
            source.Add("BMW", 179);
            source.Add("MG", 181);
            source.Add("Jaguar", 186);
            source.Add("SsangYong", 190);
            source.Add("Mercedes-Benz", 216);
            source.Add("Chrysler", 221);
            source.Add("Audi", 226);
            source.Add("Jeep", 227);
            source.Add("Alfa Romeo", 237);
            source.Add("Land Rover", 323);
            source.Add("Porsche", 365);
            source.Add("Bentley", 645);

            return source;
        }
    }
}
