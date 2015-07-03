using System.Collections.Generic;
using CarChooser.Web.Models;

namespace CarChooser.Web.Mappers
{
    public interface IMapOfficialCarRatings
    {
        OfficialRatingVM Map(KeyValuePair<string, decimal> rating);
    }

    public class OfficialCarRatingsMapper : IMapOfficialCarRatings
    {
        public OfficialRatingVM Map(KeyValuePair<string, decimal> rating)
        {
            return new OfficialRatingVM {Category = rating.Key, Score = rating.Value * 2};
        }
    }
}