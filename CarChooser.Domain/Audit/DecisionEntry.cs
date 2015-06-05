using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarChooser.Domain.Audit
{
    public class DecisionEntry
    {
        public string SessionId { get; set; }
        public int CarId { get; set; }
        public int Candidates { get; set; }
        public string DislikeReason { get; set; }
    }
}
