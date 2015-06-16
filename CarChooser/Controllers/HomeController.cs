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
        private readonly IMapSearchVMs _searchVMMapper;
        private readonly IRecordDecisions _recordDecisions;

        public HomeController(ISearchCars searchService, 
            IMapSearchRequests searchMapper, 
            IMapSearchVMs searchVMMapper,
            IRecordDecisions recordDecisions)
        {
            _searchService = searchService;
            _searchMapper = searchMapper;
            _searchVMMapper = searchVMMapper;
            _recordDecisions = recordDecisions;
        }

        [HttpGet]
        public ActionResult Index()
        {
            Session.Abandon();

            var adaptiveScorer = (AdaptiveScorer)Session["Scorer"];
            var result = _searchService.GetCar(new Search { CurrentCarId = -1 }, adaptiveScorer);

            var car = result.First();
            var model = _searchVMMapper.Map(car);

            model.CurrentCar.Image = GetBestImage(car);

            return View(model);
        }

        [HttpPost]
        public JsonResult Post(SearchRequest request)
        {
            var search = _searchMapper.Map(request);

            var adaptiveScorer = (AdaptiveScorer)Session["Scorer"];
            var currentCar = _searchService.GetCar(search, adaptiveScorer).First();
            var carProfile = CarProfile.From(currentCar);
            var like = request.LikeIt;

            adaptiveScorer.Learn(carProfile, like);

            var result = _searchService.GetCar(new Search(), adaptiveScorer).FirstOrDefault(); 

            var model = _searchVMMapper.Map(request, result, search);

            if (model.CurrentCar != null)
            {
                model.CurrentCar.Image = GetBestImage(result);
            }

            _recordDecisions.RecordDecision(new DecisionEntry()
            {
                CarId = currentCar.Id,
                DislikeReason = like ? string.Empty : "dislike",
                SessionId = Session.SessionID
            });

            return new JsonResult {Data = JsonConvert.SerializeObject(model)};
        }

        private string GetBestImage(Car model)
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
