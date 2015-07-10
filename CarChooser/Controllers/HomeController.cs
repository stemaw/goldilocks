using System.Linq;
using System.Web.Mvc;
using CarChooser.Domain;
using CarChooser.Domain.Audit;
using CarChooser.Domain.ScoreStrategies;
using CarChooser.Web.Mappers;
using CarChooser.Web.Models;
using Newtonsoft.Json;

namespace CarChooser.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISearchCars _searchService;
        private readonly IMapSearchRequests _searchMapper;
        private readonly IRecordDecisions _recordDecisions;
        private readonly IMapUserCarRatings _userCarRatingsMapper;
        private readonly IManageCars _carManager;
        private readonly IMapCars _carMapper;

        public HomeController(ISearchCars searchService, 
            IMapSearchRequests searchMapper, 
            IRecordDecisions recordDecisions,
            IMapUserCarRatings userCarRatingsMapper,
            IManageCars carManager,
            IMapCars carMapper)
        {
            _searchService = searchService;
            _searchMapper = searchMapper;
            _recordDecisions = recordDecisions;
            _userCarRatingsMapper = userCarRatingsMapper;
            _carManager = carManager;
            _carMapper = carMapper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            Session.Abandon();

            var adaptiveScorer = (AdaptiveScorer)Session["Scorer"];
            var car = _searchService.GetCar(new Search(), adaptiveScorer);

            var model = _searchMapper.Map(car);

            model.CurrentCar.Image = GetBestImage(model.CurrentCar);

            return View(model);
        }

        [HttpGet]
        public ActionResult Get(int carId)
        {
            var car = _carManager.GetCar(carId);

            var model = _searchMapper.Map(car);

            if (model.CurrentCar != null)
            {
                model.CurrentCar.Image = GetBestImage(model.CurrentCar);
            }

           return View("Index", model);
        }

        [HttpPost]
        public JsonResult Post(SearchRequestVM request)
        {
            RePopulateLikes(request);

            if (request.RequestedCarId != 0)
            {
                return GetHistoricViewModel(request);
            }

            if (request.UserRatings.Any()) SubmitRatings(request);

            var search = _searchMapper.Map(request);
            
            var adaptiveScorer = (AdaptiveScorer)Session["Scorer"];
            var currentCar = _carManager.GetCar(request.CurrentCar.Id);
            var carProfile = CarProfile.From(currentCar);
            var like = request.LikeIt;

            adaptiveScorer.Learn(carProfile, like);

            var car = _searchService.GetCar(search, adaptiveScorer); 

            var model = _searchMapper.Map(request, car, search);

            if (model.CurrentCar != null)
            {
                model.CurrentCar.Image = GetBestImage(model.CurrentCar);
            }

            _recordDecisions.RecordDecision(new DecisionEntry()
            {
                CarId = currentCar.Id,
                DislikeReason = like ? string.Empty : "dislike",
                SessionId = Session.SessionID
            });

            return new JsonResult {Data = JsonConvert.SerializeObject(model)};
        }

        private void RePopulateLikes(SearchRequestVM request)
        {
            if (request.Likes == null) return;

            for (var i = 0; i < request.Likes.Count; i++)
            {
                request.Likes[i] = _carMapper.Map(_carManager.GetCar(request.Likes[i].Id));
                request.Likes[i].Image = GetBestImage(request.Likes[i]);
            }
        }

        [HttpPost]
        public void ReportProblem(int id, string reason)
        {
            _carManager.ReportProblem(id, reason);
        }

        private void SubmitRatings(SearchRequestVM request)
        {
            var car = _carManager.GetCar(request.CurrentCar.Id);

            car.UserRatings.Add(request.UserRatings.Select(r => _userCarRatingsMapper.Map(r)).ToList());

            _carManager.UpdateRatings(car);
        }

        private JsonResult GetHistoricViewModel(SearchRequestVM request)
        {
            var car = _carManager.GetCar(request.RequestedCarId);

            var search = _searchMapper.Map(request);

            var model = _searchMapper.Map(request, car, search);

            if (model.CurrentCar != null)
            {
                model.CurrentCar.Image = GetBestImage(model.CurrentCar);
            }

            return new JsonResult { Data = JsonConvert.SerializeObject(model) };
        }

        private string GetBestImage(CarVM model)
        {
            const string imageRoot = "/content/carimages/";
            var physicalRoot = Server.MapPath(imageRoot);
       
            const string derivativeFormat = "{0}{1}-{2}.jpg";
            var derivativePath = string.Format(derivativeFormat, physicalRoot, model.ModelId, model.Id);
            
            if (System.IO.File.Exists(derivativePath)) return string.Format(derivativeFormat, imageRoot, model.ModelId, model.Id);

            const string modelFormat = "{0}{1}.jpg";
            var modelPath = string.Format(modelFormat, physicalRoot, model.ModelId);

            if (System.IO.File.Exists(modelPath)) return string.Format(modelFormat, imageRoot, model.ModelId);

            return imageRoot + "default.png";
        }
    }
}
