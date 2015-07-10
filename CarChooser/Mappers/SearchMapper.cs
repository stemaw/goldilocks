using System;
using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain;
using CarChooser.Web.Models;

namespace CarChooser.Web.Mappers
{
    public class SearchMapper : IMapSearchRequests
    {
        private readonly IMapCars _carMapper;

        public SearchMapper(IMapCars carMapper)
        {
            _carMapper = carMapper;
        }

        public Search Map(SearchRequestVM request)
        {
            return new Search
            {
                CurrentCar = _carMapper.Map(request.CurrentCar),
                Likes = GetLikes(request).Select(l => l.Id).ToList(),
                PreviousRejections = GetRejections(request),
            };
        }

        public SearchResultVM Map(SearchRequestVM request, Car result, Search search)
        {
            return new SearchResultVM
            {
                CurrentCar = _carMapper.Map(result),
                Likes = request.Likes,
                PreviousRejections = search.PreviousRejections.Select(r =>
                                                              new RejectionVM
                                                              {
                                                                  Reason = r.Reason.ToString(),
                                                                  CarId = r.CarId,
                                                              }
                    ).ToList()
            };
        }

        public SearchResultVM Map(Car result)
        {
            return new SearchResultVM
            {
                Likes = new List<CarVM>(),
                CurrentCar = _carMapper.Map(result),
            };
        }

        private static List<PreviousRejection> GetRejections(SearchRequestVM request)
        {
            if (request.PreviousRejections != null)
            {
                var previous = request.PreviousRejections.Select(r => new PreviousRejection {CarId = r.CarId}).ToList();
                if (!request.LikeIt)
                {
                    previous.Add(new PreviousRejection() {CarId = request.CurrentCar.Id});
                }
                return previous;
            }

            return null;
        }

        private static IEnumerable<CarVM> GetLikes(SearchRequestVM request)
        {
            if (request.Likes == null)
            {
                request.Likes = new List<CarVM>();
            }

            if (request.LikeIt)
            {
                request.Likes.Add(request.CurrentCar);
            }

            return request.Likes;
        }
    }

    public interface IMapSearchRequests
    {
        Search Map(SearchRequestVM request);
        SearchResultVM Map(SearchRequestVM request, Car result, Search search);
        SearchResultVM Map(Car result);
    }
}