using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    public class BoardAlignmentConfiguration
    {

        public BoardAlignmentConfiguration()
        {
            Roll = 0;
            Pitch = 0;
            Yaw = 0;
        }


        public float Roll { get; internal set; }
        public float Pitch { get; internal set; }
        public float Yaw { get; internal set; }
    }
}
