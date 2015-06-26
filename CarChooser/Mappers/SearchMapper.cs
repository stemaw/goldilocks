using System;
using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain;
using CarChooser.Web.Models;

namespace CarChooser.Web.Mappers
{
    public class SearchMapper : IMapSearchRequests
    {
        private readonly IMapCarVMs _carMapper;

        public SearchMapper(IMapCarVMs carMapper)
        {
            _carMapper = carMapper;
        }

        public Search Map(SearchRequest request)
        {
            return new Search
            {
                CurrentCar = _carMapper.Map(request.CurrentCar),
                Likes = GetLikes(request).Select(l => l.Id).ToList(),
                PreviousRejections = GetRejections(request),
            };
        }

        private static List<PreviousRejection> GetRejections(SearchRequest request)
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

        private static IEnumerable<CarVM> GetLikes(SearchRequest request)
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
        Search Map(SearchRequest request);
    }
}