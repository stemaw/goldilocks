using System;
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

        public Car GetCar(Search search, IFilter judge)
        {
            if (search.CurrentCarId > 0)
                return _carRepository.GetCar(search.CurrentCarId);
            if (search.CurrentCarId < 0)
            {
                return _carRepository.GetDefaultCar();
            }

            SetMinPerformance(search);
            SetMinPrestige(search);
            SetMinReliability(search);
            SetMinAttractiveness(search);
            SetMinSize(search);
            SetMaxPrice(search);

            Func<Car, bool> predicate =
                c => true;

            var concreteOptions = _carRepository.GetCars(predicate).ToList();
            var matches = judge.Filter(concreteOptions);

            var seed = concreteOptions.Count;

            if ((matches != null) && (matches.Any()))
                seed = matches.Count;
            else
                matches = concreteOptions;

            return matches.Skip(new Random().Next(seed)).Take(1).First();
        }


        private void SetMaxPrice(Search search)
        {
            var previousRejection = search.PreviousRejections.FirstOrDefault(r => r.Reason == RejectionReasons.TooExpensive);

            search.MaxPriceScore = previousRejection == null ? 100 : _carRepository.GetCar(previousRejection.CarId).PriceScore;
        }

        private void SetMinSize(Search search)
        {
            var previousRejection = search.PreviousRejections.FirstOrDefault(r => r.Reason == RejectionReasons.TooSmall);

            search.MinSizeScore = previousRejection == null ? 0 : _carRepository.GetCar(previousRejection.CarId).SizeScore;
        }

        private void SetMinAttractiveness(Search search)
        {
            var previousRejection = search.PreviousRejections.FirstOrDefault(r => r.Reason == RejectionReasons.TooUgly);

            search.MinAttractivenessScore = previousRejection == null ? 0 : _carRepository.GetCar(previousRejection.CarId).AttractivenessScore;
        }
        
        private void SetMinReliability(Search search)
        {
            var previousRejection = search.PreviousRejections.FirstOrDefault(r => r.Reason == RejectionReasons.TooUnreliable);

            search.MinReliabilityScore = previousRejection == null ? 0 : _carRepository.GetCar(previousRejection.CarId).ReliabilityScore;
        }
        
        private void SetMinPrestige(Search search)
        {
            var previousRejection = search.PreviousRejections.FirstOrDefault(r => r.Reason == RejectionReasons.TooCommon);

            search.MinPrestigeScore = previousRejection == null ? 0 : _carRepository.GetCar(previousRejection.CarId).PrestigeScore;
        }

        private void SetMinPerformance(Search search)
        {
            var previousRejection = search.PreviousRejections.FirstOrDefault(r => r.Reason == RejectionReasons.TooSlow);

            search.MinPerformance = previousRejection == null ? 0 : _carRepository.GetCar(previousRejection.CarId).PerformanceScore;
        }
    }
}
