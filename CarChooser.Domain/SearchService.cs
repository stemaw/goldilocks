using System;
using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain.ScoreStrategies;

namespace CarChooser.Domain
{
    public class SearchService : ISearchCars
    {
        private readonly IGetCars _carRepository;
        readonly Random _random;

        public SearchService(IGetCars carRepository)
        {
            _carRepository = carRepository;
            _random = new Random();
        }

        public IEnumerable<Car> GetCar(Search search, IFilter judge)
        {
            if (search.CurrentCar == null)
            {
                return new List<Car> { _carRepository.GetDefaultCar() };
            }

            var concreteOptions = _carRepository.AllCars().ToList();
            var matches = judge.Filter(concreteOptions);

            if (matches == null || !matches.Any())
            {
                matches = concreteOptions;
            }

            var otherModels = matches.Where(m => m.Model != search.CurrentCar.Model).ToList();
            
            if (otherModels.Any() && otherModels.Count > 25)
            {
                matches = otherModels;
            }
            
            var seed = matches.Count;

            const int stopCount = 25;
            
            return seed <= stopCount ? new List<Car>() : matches.Skip(_random.Next(seed)).Take(1);
        }

        public Car GetCar(int id)
        {
            return _carRepository.GetCar(id);
        }
    }
}
