using System;
using System.Collections.Generic;

namespace CarChooser.Web.Models
{
    public class SearchResultVM
    {
        public CarVM CurrentCar { get; set; }
        public IEnumerable<CarVM> Likes { get; set; }
        public IEnumerable<CarVM> Dislikes { get; set; }

        public IEnumerable<string> RejectionReasons{get { return Enum.GetNames(typeof(Domain.RejectionReasons)); }}

        public IEnumerable<RejectionVM> PreviousRejections { get; set; } 
    }
}