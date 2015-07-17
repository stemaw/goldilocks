using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarChooser.Domain;
using CarChooser.Web.Mappers;
using CarChooser.Web.Models;
using Newtonsoft.Json;

namespace CarChooser.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly IManageCars _carManager;
        private readonly IMapCars _carMapper;
        private readonly Random _random;

        public QuizController(IManageCars carManager, IMapCars carMapper)
        {
            _carManager = carManager;
            _carMapper = carMapper;
            _random = new Random();
        }

        public ActionResult Index()
        {
            var model = GetQuestionModel();

            return View(model);
        }

        private GameModel GetQuestionModel()
        {
            var allCars = _carManager.GetAllCars().ToList();

            var question = Questions().Skip(_random.Next(Questions().Count)).First();

            var elligibleCars = allCars.Where(question.Requires).ToList();
            var carCount = elligibleCars.Count();

            var carOne = _carMapper.Map(elligibleCars.Skip(_random.Next(carCount)).First());
            var carTwo = _carMapper.Map(elligibleCars.Skip(_random.Next(carCount)).First());

            var model = new GameModel
                {
                    Cars = new List<CarVM> {carOne, carTwo},
                    Question = question.QuestionText,
                    Answer = question.Answer(carOne, carTwo).Id,
                    CarOneAnswer = question.ValueToShow(carOne),
                    CarTwoAnswer = question.ValueToShow(carTwo)
                };
            return model;
        }

        public JsonResult Post(GameModel model)
        {
            // learn something

            var newModel = GetQuestionModel();
            newModel.Score = model.Score;
            newModel.QuestionsAsked = model.QuestionsAsked + 1;
            if (model.Correct != null && (bool)model.Correct) newModel.Score++;
            
            return new JsonResult { Data = JsonConvert.SerializeObject(newModel) };
        }

        private List<Question> Questions()
        {
            return new List<Question>()
                {
                    new Question
                        {
                            QuestionText = "Which has the fastest acceleration?",
                            Answer = (a,b) => a.Acceleration < b.Acceleration ? a : b,
                            Requires = c => c.Acceleration > 0,
                            ValueToShow = c => string.Format("{0} secs", c.Acceleration)
                        },
                    new Question
                        {
                            QuestionText = "Which does the most miles to the gallon?",
                            Answer = (a,b) => a.Mpg > b.Mpg ? a : b,
                            Requires = c => c.Mpg > 0,
                                ValueToShow = c => string.Format("{0} mpg", c.Mpg)
                        },
                    new Question
                        {
                            QuestionText = "Which has the highest top speed?",
                            Answer = (a,b) => a.TopSpeed > b.TopSpeed ? a : b,
                            Requires = c => c.TopSpeed > 0,
                            ValueToShow = c => string.Format("{0} mph", c.TopSpeed)
                        },
                    new Question
                        {
                            QuestionText = "Which is most expensive?",
                            Answer = (a,b) => a.Price > b.Price ? a : b,
                            Requires = c => c.Price > 0,
                            ValueToShow = c => string.Format("{0:C0}", c.Price)
                        },
                    new Question
                        {
                            QuestionText = "Which has the biggest boot?",
                            Answer = (a,b) => a.LuggageCapacity > b.LuggageCapacity ? a : b,
                            Requires = c => c.LuggageCapacity > 0,
                            ValueToShow = c => string.Format("{0} litres", c.LuggageCapacity)
                        },
                    new Question
                        {
                            QuestionText = "Which has the most BHP (power)?",
                            Answer = (a,b) => a.Power > b.Power ? a : b,
                            Requires = c => c.Power > 0,
                            ValueToShow = c => string.Format("{0} bhp", c.Power)
                        },
                    new Question
                        {
                            QuestionText = "Which is the most expensive to insure?",
                            Answer = (a,b) => a.InsuranceGroup > b.InsuranceGroup ? a : b,
                            Requires = c => c.InsuranceGroup > 0,
                            ValueToShow = c => string.Format("group {0}", c.InsuranceGroup)
                        }
                };
        }
	}

    public class Question
    {
        public string QuestionText { get; set; }
        public Func<CarVM, CarVM, CarVM> Answer { get; set; }
        public Func<Car, bool> Requires { get; set; }
        public Func<CarVM, string> ValueToShow { get; set; }
    }

    public class GameModel
    {
        public List<CarVM> Cars { get; set; }

        public string Question { get; set; }

        public int Answer { get; set; }

        public int Score { get; set; }

        public bool? Correct { get; set; }

        public int QuestionsAsked { get; set; }

        public string CarOneAnswer { get; set; }

        public string CarTwoAnswer { get; set; }
    }
}