using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    public class FeatureConfiguration
    {

        public FeatureConfiguration()
        {
            Features = 0;
        }


        public int Features { get; internal set; }

    }
}
