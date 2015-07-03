using System.Collections.Generic;
using System.Linq;

namespace CarChooser.Domain
{
    public class CarService : IManageCars
    {
        private readonly IGetCars _carRepository;
        private readonly IReportProblems _reporter;

        public CarService(IGetCars carRepository, IReportProblems reporter)
        {
            _carRepository = carRepository;
            _reporter = reporter;
        }

        public IEnumerable<Car> GetAllCars()
        {
            return _carRepository.AllCars().ToList();
        }

        public IEnumerable<Car> GetAllCars(int pageNumber)
        {
            const int pageSize = 10;
            var skip = (pageNumber-1)*pageSize;
            return _carRepository.AllCars(skip, pageSize).ToList();
        }

        public void UpdateRatings(Car car)
        {
            _carRepository.Save(car);
        }

        public Car GetCar(int id)
        {
            return _carRepository.GetCar(id);
        }

        public void ReportProblem(int id, string reason)
        {
            _reporter.Report(reason, id);
        }
    }
}
