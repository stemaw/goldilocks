using System.Collections.Generic;

namespace CarChooser.Domain
{
    public class Search
    {
        private List<PreviousRejection> _previousRejections;
        
        public long CurrentCarId { get; set; }
        
        public RejectionReasons? RejectionReason { get; set; }
        
        public IEnumerable<long> Likes { get; set; }
        
        public IEnumerable<long> Dislikes { get; set; }

        public int MinPerformance { get; set; }

        public int MinPrestigeScore { get; set; }

        public int MinReliabilityScore { get; set; }

        public int MinAttractivenessScore { get; set; }

        public int MinSizeScore { get; set; }

        public int MaxPriceScore { get; set; }

        public List<PreviousRejection> PreviousRejections
        {
            get { return _previousRejections ?? (_previousRejections = new List<PreviousRejection>()); }
            set { _previousRejections = value; }
        }
    }
}