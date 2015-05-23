using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CarChooser.Domain;
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
        private readonly IPresentCars _carFilter;

        public HomeController(ISearchCars searchService, 
            IMapSearchRequests searchMapper, 
            IMapSearchVMs searchVMMapper)
        {
            _searchService = searchService;
            _searchMapper = searchMapper;
            _searchVMMapper = searchVMMapper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var result = _searchService.GetCar(new Search());
            
            var model = _searchVMMapper.Map(result);
            
            return View(model);
        }

        [HttpPost]
        public JsonResult Post(SearchRequest request)
        {
            var search = _searchMapper.Map(request);

            var adaptiveScorer = (AdaptiveScorer)Session["AdaptiveScorere"];
            var performanceVms = request.CurrentCar.Performance;

            var carProfile = new CarProfile(request.CurrentCar.Manufacturer, 
                request.CurrentCar.Model, 
                new Dictionary<string, double>());

            foreach( var perfIndicator in performanceVms )
            {
                carProfile.Characteristics.Add("Power", perfIndicator.Power);
                carProfile.Characteristics.Add("Top Speed", perfIndicator.TopSpeed);
            }

            //carProfile.Characteristics.Add("Doors", request.CurrentCar.GetDoorCount());

            bool like = ( request.RejectionReason != string.Empty );

            adaptiveScorer.Learn(carProfile, like);

            var result = _searchService.GetCar(search);

            var model = _searchVMMapper.Map(request, result, search);

            return new JsonResult {Data = JsonConvert.SerializeObject(model)};
        }     
    }
}
