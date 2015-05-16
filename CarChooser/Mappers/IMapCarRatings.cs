using System.Collections.Generic;

namespace CarChooser.Web.Mappers
{
    public interface IMapCarRatings
    {
        RatingVM Map(KeyValuePair<string, decimal> rating);
    }
}