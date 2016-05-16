using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CarChooser.Domain;

namespace CarChooser.Web.Controllers
{
    public class StatsController : Controller
    {
        private readonly IManageCars _carManager;

        public StatsController(IManageCars carManager)
        {
            _carManager = carManager;
        }

        public ViewResult Index(string manufacturer)
        {
            var allDerivatives =
                _carManager.GetAllCars()
                    .Where(
                        c => string.Equals(Format(c.Manufacturer.Name), manufacturer, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

            if (!allDerivatives.Any()) throw new HttpException(404, "Manfuacturer not found");
            
            var manufacturerInfo = allDerivatives.First().Manufacturer;

            var models = from car in allDerivatives
                         group car by new { car.Model, car.YearFrom, car.YearTo }
                             into g
                             select new { Model = g.Key, Derivates = g.ToList() };

            var model = new ManufacturerStatsModel()
            {
                Manufacturer = HttpUtility.HtmlDecode(manufacturerInfo.Name),
                Cars = models.OrderBy(c => c.Model.Model).ThenBy(c => c.Model.YearFrom).Select(c => Map(c.Model.Model, c.Model.YearFrom, c.Model.YearTo, c.Derivates)).ToList(),
                
            };

            return View(model);
        }

        private string Format(string name)
        {
            return HttpUtility.HtmlEncode(name.Replace(" ", ""));
        }

        private ManufacturerCarPerformanceStatsModel Map(string model, int yearFrom, int yearTo, IEnumerable<Car> derivatives)
        {
            return new ManufacturerCarPerformanceStatsModel()
            {
                Model = model,
                Year = CarHelper.GetYear(yearFrom, yearTo),
                Derivatives = derivatives.Select(d => new ManufacturerCarPerformanceStatsDerivativeModel
                {
                    Acceleration = d.Acceleration,
                    Mpg = d.Mpg,
                    TopSpeed = d.TopSpeed,
                    Name = d.Name,
                    Url = CarHelper.GetUrl(d),
                }).ToList()
            };
        }
    }

    public class ManufacturerStatsModel
    {
        public string Manufacturer { get; set; }
        public IEnumerable<ManufacturerCarPerformanceStatsModel> Cars { get; set; }
    }

    public class ManufacturerCarPerformanceStatsModel
    {
        public string Model { get; set; }
        public string Year { get; set; }
        public IEnumerable<ManufacturerCarPerformanceStatsDerivativeModel> Derivatives { get; set; } 
    }

    public class ManufacturerCarPerformanceStatsDerivativeModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public decimal Acceleration { get; set; }
        public int TopSpeed { get; set; }
        public int Mpg { get; set; }
    }
}
