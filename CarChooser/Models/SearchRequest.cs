using System;
using System.Collections.Generic;

namespace CarChooser.Web.Models
{
    public class SearchRequest
    {
        public Guid CurrentSession { get; set; }
        public CarVM CurrentCar { get; set; }
        public string RejectionReason { get; set; }
        public IList<CarVM> Likes { get; set; }
        public IList<CarVM> Dislikes { get; set; }
        public IList<RejectionVM> PreviousRejections { get; set; }
        public bool LikeIt {get ; set; }
    }
}