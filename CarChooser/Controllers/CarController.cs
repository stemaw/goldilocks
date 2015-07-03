using System.Linq;
using System.Web.Mvc;
using CarChooser.Domain;
using CarChooser.Web.Mappers;
using Newtonsoft.Json;

namespace CarChooser.Web.Controllers
{
    public class CarController : Controller
    {
        private readonly IManageCars _carService;
        private readonly IMapCars _carMapper;

        public CarController(IManageCars carService, IMapCars carMapper)
        {
            _carService = carService;
            _carMapper = carMapper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Get(int pageNumber)
        {
            var cars = _carService.GetAllCars(pageNumber);

            var model = cars.Select(c => _carMapper.Map(c));

            return new JsonResult {Data = JsonConvert.SerializeObject(model), JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }
    }
}
