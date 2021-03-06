﻿using System;
using System.Collections.Generic;
using System.Linq;

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

        public Car GetCar(Search search, IFilter judge)
        {
            if (search.CurrentCar == null)
            {
                return GetInitialCar();
            }

            var concreteOptions = _carRepository.AllCars();
            var matches = judge.Filter(concreteOptions);

            if (matches == null || !matches.Any())
            {
                matches = concreteOptions.OrderByDescending(c => judge.ScoreTheCar(c.Profile)).ToList();
            }

            var previouslySeen = GetPreviousSeenModels(search);
            var otherModels = matches.Where(m => !previouslySeen.Contains(m.Model)).ToList();

            var stopCount = 25;

            if (otherModels.Any())
            {
                matches = otherModels;
                stopCount = 1;
            }
            
            var seed = (int)Math.Floor(matches.Count/10D);

            return seed < stopCount ? null : matches.Skip(_random.Next(seed)).Take(1).ElementAt(0);
        }

        private Car GetInitialCar()
        {
            return _carRepository.AllCars().Skip(_random.Next(_carRepository.AllCars().Count)).Take(1).First();
        }

        private List<string> GetPreviousSeenModels(Search search)
        {
            return search.PreviousRejections.Select(rejectionId => _carRepository.GetCar(rejectionId.CarId).Model)
                .Union(search.Likes.Select(id => _carRepository.GetCar(id).Model))
                .ToList();
        }
    }
}
