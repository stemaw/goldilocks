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
    public class ManufacturerListController : Controller
    {
        private readonly IManageCars _carManager;

        public ManufacturerListController(IManageCars carManager)
        {
            _carManager = carManager;
        }

        public ViewResult Index()
        {
            var manufacturers = _carManager.GetAllCars().GroupBy(c => c.Manufacturer.Name).Select(g => g.Key);

            var model = new ManufacturerListModel()
            {
                Manufacturers = manufacturers.OrderBy(m => m).Select(Map)
            };

            return View(model);
        }

        private ManufacturerItemModel Map(string name)
        {
            return new ManufacturerItemModel()
            {
                Name = HttpUtility.HtmlEncode(name),
                Logo = name + ".png",
                Url = string.Concat("manufacturer/", Format(name)),
                StatsUrl = string.Concat("stats/", Format(name))
            };
        }

        private string Format(string name)
        {
            return HttpUtility.UrlEncode(name.Replace(" ", ""));
        }
    }

    public class ManufacturerListModel
    {
        public IEnumerable<ManufacturerItemModel> Manufacturers { get; set; }
    }

    public class ManufacturerItemModel
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Url { get; set; }
        public string StatsUrl { get; set; }
    }
}
