using System;
using System.Linq;

namespace CarChooser.Domain
{
    public class SearchService : ISearchCars
    {
        private readonly IGetCars _carRepository;

        public SearchService(IGetCars carRepository)
        {
            _carRepository = carRepository;
        }

        public Car GetCar(Search search)
        {
            if (search.CurrentCarId == 0) return _carRepository.GetDefaultCar();

            var currentCar = _carRepository.GetCar(search.CurrentCarId);

            search.PreviousRejections.Add(new PreviousRejection{ CarId = search.CurrentCarId, Reason = search.RejectionReason});

            SetMinPerformance(search);
            SetMinPrestige(search);
            SetMinReliability(search);
            SetMinAttractiveness(search);
            SetMinSize(search);
            SetMaxPrice(search);

            Func<Car, bool> predicate =
                c => c.PerformanceScore >= search.MinPerformance
                     && c.PrestigeScore >= search.MinPrestigeScore
                     && c.ReliabilityScore >= search.MinReliabilityScore
                     && c.AttractivenessScore >= search.MinAttractivenessScore
                     && c.SizeScore >= search.MinSizeScore
                     && c.PriceScore <= search.MaxPriceScore
                     && !search.PreviousRejections.Select(r => r.CarId).Contains(c.Id)
                ;

            var matches = _carRepository.GetCars(predicate)
                                        .OrderBy(CalculateTotalScore);

            return matches.Any() ? matches.First() : currentCar;
        }

        private static int CalculateTotalScore(Car car)
        {
            return car.AttractivenessScore + car.PerformanceScore + car.PrestigeScore + car.PriceScore + car.SizeScore +
                   car.ReliabilityScore;
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
