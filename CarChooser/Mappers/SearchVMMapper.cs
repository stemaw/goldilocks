using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain;
using CarChooser.Web.Models;

namespace CarChooser.Web.Mappers
{
    public class SearchVMMapper : IMapSearchVMs
    {
        private readonly IMapCarVMs _carMapper;

        public SearchVMMapper(IMapCarVMs carMapper)
        {
            _carMapper = carMapper;
        }

        public SearchResultVM Map(SearchRequest request, Car result, Search search)
        {
            return new SearchResultVM
                {
                    CurrentCar = _carMapper.Map(result),
                    Dislikes = request.Dislikes,
                    Likes = request.Likes,
                    PreviousRejections = search.PreviousRejections.Select(r =>
                                                                  new RejectionVM
                                                                      {
                                                                          Reason = r.Reason.ToString(),
                                                                          CarId = r.CarId,
                                                                      }
                        )
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
    }

    public interface IMapSearchVMs
    {
        SearchResultVM Map(SearchRequest request, Car result, Search search);
        SearchResultVM Map(Car result);
    }
}