using System.Collections.Generic;

namespace CarChooser.Web.Mappers
{
    public class CarRatingsMapper : IMapCarRatings
    {
        public RatingVM Map(KeyValuePair<string, decimal> rating)
        {
            return new RatingVM {Category = rating.Key, Score = rating.Value * 2};
        }
    }
}