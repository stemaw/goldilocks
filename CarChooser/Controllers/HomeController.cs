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
            var adaptiveScorer = (AdaptiveScorer)Session["Scorer"];
            var result = _searchService.GetCar(new Search { CurrentCarId = -1 }, adaptiveScorer);
            
            var model = _searchVMMapper.Map(result);
            
            return View(model);
        }

        [HttpPost]
        public JsonResult Post(SearchRequest request)
        {
            var search = _searchMapper.Map(request);

            var adaptiveScorer = (AdaptiveScorer)Session["Scorer"];
            var currentCar = _searchService.GetCar(search, adaptiveScorer);
            var carProfile = CarProfile.From(currentCar);
            var like = request.LikeIt;

            adaptiveScorer.Learn(carProfile, like);

            var result = _searchService.GetCar(new Search(), adaptiveScorer); 

            var model = _searchVMMapper.Map(request, result, search);

            _recordDecisions.RecordDecision(new DecisionEntry()
            {
                CarId = currentCar.Id,
                DislikeReason = like ? string.Empty : "dislike",
                SessionId = Session.SessionID
            });

            return new JsonResult {Data = JsonConvert.SerializeObject(model)};
        }     
    }
}
