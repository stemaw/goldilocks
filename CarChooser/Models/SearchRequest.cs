using System;
using System.Collections.Generic;

namespace CarChooser.Web.Models
{
    public class SearchRequest
    {
        public Guid CurrentSession { get; set; }
        public CarVM CurrentCar { get; set; }
        public string RejectionReason { get; set; }
        public IEnumerable<CarVM> Likes { get; set; }
        public IEnumerable<CarVM> Dislikes { get; set; }
        public IEnumerable<RejectionVM> PreviousRejections { get; set; }
    }
}