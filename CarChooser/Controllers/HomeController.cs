﻿using System.Linq;
using System.Web;
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

            return View(model);
        }

        [HttpGet]
        public ActionResult Get(int carId)
        {
            var car = _carManager.GetCar(carId);

            var model = _searchMapper.Map(car);

           return View("Index", model);
        }

        [HttpGet]
        public JsonResult GetManufacturers()
        {
            var manufacturers = (from car in _carManager.GetAllCars()
                                group car by car.Manufacturer.Name
                                into grp
                                select HttpUtility.HtmlDecode(grp.Key)).ToList();

            return new JsonResult { Data = JsonConvert.SerializeObject(manufacturers), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetModels(string manufacturer)
        {
            var models = (from car in _carManager.GetAllCars()
                         where car.Manufacturer.Name == manufacturer
                         group  car by car.Model into grp
                         select grp.Key).ToList();

            return new JsonResult { Data = JsonConvert.SerializeObject(models), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [HttpGet]
        public JsonResult GetDerivatives(string model)
        {
            var models = (from car in _carManager.GetAllCars()
                          where car.Model == model
                          select new { Id = car.Id, Name = car.DerivativeName, Year = car.GetYear() }).ToList();

            return new JsonResult { Data = JsonConvert.SerializeObject(models), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

            return new JsonResult { Data = JsonConvert.SerializeObject(model) };
        }
    }
}
