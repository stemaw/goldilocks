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
                //RejectionReason = !request.LikeIt ? (RejectionReasons)Enum.Parse(typeof(RejectionReasons), request.RejectionReason) : (RejectionReasons?) null,
                CurrentCar = _carMapper.Map(request.CurrentCar),
                Dislikes = request.Dislikes == null ? new int[0] : request.Dislikes.Select(c => c.Id),
                Likes = GetLikes(request).Select(l => l.Id).ToList(),
                PreviousRejections = request.PreviousRejections != null ? request.PreviousRejections.Select(r => 
                new PreviousRejection
                    {
                        CarId = r.CarId,
                        //Reason = (RejectionReasons) Enum.Parse(typeof(RejectionReasons), r.Reason)
                    }).ToList() : null
            };
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