using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    public class MixerConfiguration
    {
        public MixerConfiguration()
        {
            Mixer = 0;
        }

        public int Mixer { get; internal set; }
    }
}
