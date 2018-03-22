using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    public class RCTuning
    {

        public RCTuning()
        {
            RCRate = 0.0f;
            RCExpo = 0.0f;
            RollPitchRate = 0.0f;
            RollRate = 0.0f;
            PitchRate = 0.0f;
            YawRate = 0.0f;
            DynamicTHRPID = 0.0f;
            ThrotleExpo = 0.0f;
            DynamicTHRBreakpoint = 0.0f;
            RCYawExpo = 0.0f;
            RCYawRate = 0.0f;
        }


        public float RCRate { get; internal set; }
        public float RCExpo { get; internal set; }
        public float RollPitchRate { get; internal set; }   // Pre 1.7 api only
        public float RollRate { get; internal set; }
        public float PitchRate { get; internal set; }
        public float YawRate { get; internal set; }
        public float DynamicTHRPID { get; internal set; }
        public float ThrotleExpo { get; internal set; }
        public float DynamicTHRBreakpoint { get; internal set; }
        public float RCYawExpo { get; internal set; }
        public float RCYawRate { get; internal set; }

    }
}
