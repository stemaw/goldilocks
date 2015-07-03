using CarChooser.Domain;
using CarChooser.Web.Models;

namespace CarChooser.Web.Mappers
{
    public interface IMapUserCarRatings
    {
        UserRatingVM Map(UserRating rating);
        UserRating Map(UserRatingVM rating);
    }

    public class UserCarRatingMapper : IMapUserCarRatings
    {
        public UserRatingVM Map(UserRating rating)
        {
            return new UserRatingVM
                {
                    Characteristic = rating.Characteristic,
                    Score = rating.Score
                };
        }

        public UserRating Map(UserRatingVM rating)
        {
            return new UserRating
            {
                Characteristic = rating.Characteristic,
                Score = rating.Score
            };
        }
    }
}