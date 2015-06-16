using System;
using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain.ScoreStrategies;

namespace CarChooser.Domain
{
    public class SearchService : ISearchCars
    {
        private readonly IGetCars _carRepository;

        public SearchService(IGetCars carRepository)
        {
            _carRepository = carRepository;
        }

        public IEnumerable<Car> GetCar(Search search, IFilter judge)
        {
            if (search.CurrentCarId > 0)
                return new List<Car> {_carRepository.GetCar(search.CurrentCarId)};
            
            if (search.CurrentCarId < 0)
            {
                return new List<Car> {_carRepository.GetDefaultCar()};
            }
           
            var concreteOptions = _carRepository.AllCars().ToList();
            var matches = judge.Filter(concreteOptions);

            if (matches == null || !matches.Any())
            {
                matches = concreteOptions;
            }

            var seed = matches.Count;

            const int stopCount = 25;

            return seed <= stopCount ? null : matches.Skip(new Random().Next(matches.Count)).Take(1);
        }
    }
}
