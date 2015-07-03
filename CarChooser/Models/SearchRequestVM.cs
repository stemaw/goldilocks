using System;
using System.Collections.Generic;

namespace CarChooser.Web.Models
{
    public class SearchRequestVM
    {
        public Guid CurrentSession { get; set; }
        public CarVM CurrentCar { get; set; }
        public string RejectionReason { get; set; }
        public IList<CarVM> Likes { get; set; }
        public IList<RejectionVM> PreviousRejections { get; set; }
        public bool LikeIt {get ; set; }
        public int RequestedCarId { get; set; }
        public IList<UserRatingVM> UserRatings { get; set; } 
    }
}