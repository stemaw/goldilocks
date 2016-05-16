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
    public class ManufacturerController : Controller
    {
        private readonly IManageCars _carManager;

        public ManufacturerController(IManageCars carManager)
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
                         group car by new { car.Model, car.YearFrom}
                             into g
                             select new { Model = g.Key.Model, Derivates = g.ToList() };

            var model = new ManufacturerModel()
            {
                Name = HttpUtility.HtmlDecode(manufacturerInfo.Name),
                Reliability = manufacturerInfo.ReliabilityIndex,
                Prestige = manufacturerInfo.PrestigeIndex,
                Cars = models.OrderBy(c => c.Model).Select(c => Map(c.Derivates.First())),
                NumberOfDerivatives = models.Sum(m => m.Derivates.Count)
            };

            return View(model);
        }

        private string Format(string name)
        {
            return HttpUtility.HtmlEncode(name.Replace(" ", ""));
        }

        private ManufacturerCarModel Map(Car car)
        {
            return new ManufacturerCarModel()
            {
                Name = car.Model,
                Image = CarHelper.GetBestImage(car.ModelId, car.Id),
                Url = CarHelper.GetUrl(car),
                Year = CarHelper.GetYear(car.YearFrom, car.YearTo)
            };
        }
    }

    public class ManufacturerModel
    {
        public string Name { get; set; }
        public int Reliability { get; set; }
        public int Prestige { get; set; }
        public IEnumerable<ManufacturerCarModel> Cars { get; set; }
        public int NumberOfDerivatives { get; set; }
    }

    public class ManufacturerCarModel
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string Year { get; set; }
    }
}
