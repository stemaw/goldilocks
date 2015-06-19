using System.Collections.Generic;

namespace CarChooser.Domain
{
    public class Search
    {
        private List<PreviousRejection> _previousRejections;
        
        public Car CurrentCar { get; set; }
        
        public RejectionReasons? RejectionReason { get; set; }
        
        public IEnumerable<int> Likes { get; set; }
        
        public IEnumerable<int> Dislikes { get; set; }

        public List<PreviousRejection> PreviousRejections
        {
            get { return _previousRejections ?? (_previousRejections = new List<PreviousRejection>()); }
            set { _previousRejections = value; }
        }
    }
}