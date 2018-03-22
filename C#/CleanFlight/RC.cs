using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    public class RC
    {

        public RC()
        {
            Channels = new float[16];
        }

        public float[] Channels { get; internal set; }
    }
}
