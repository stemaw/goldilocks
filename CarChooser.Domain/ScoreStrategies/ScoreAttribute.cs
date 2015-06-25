using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarChooser.Domain.ScoreStrategies
{
    public class ScoreAttribute : Attribute
    {
        public ScoreAttribute(double weighting = 1)
        {
        }
    }
}
