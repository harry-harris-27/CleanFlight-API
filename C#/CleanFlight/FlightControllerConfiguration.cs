using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    public class FlightControllerConfiguration
    {
        public FlightControllerConfiguration()
        {
            APIVersion = "0.0.0";
            FlightControllerIdentifier = "";
            FlightControllerVersion = "";
            Version = 0;
            BuildInfo = "";
            MultiInfo = 0;
            MSPVersion = 0;
            Capability = 0;
            CycleTime = 0;
            I2CError = 0;
            ActiveSensors = 0;
            Mode = 0;
            Profile = 0;
            UId = new int[3];
            AccelerometerTrims = new int[2];
            Name = "";
            NumberofProfiles = 0;
            RateProfile = 0;
            BoardType = 0;
        }


        public string APIVersion { get; internal set; }

        public string FlightControllerIdentifier { get; internal set; }

        public string FlightControllerVersion { get; internal set; }

        public int Version { get; internal set; }

        public string BuildInfo { get; internal set; }

        public int MultiInfo { get; internal set; }

        public int MSPVersion { get; internal set; }

        public int Capability { get; internal set; }

        public int CycleTime { get; internal set; }

        public int I2CError { get; internal set; }

        public int ActiveSensors { get; internal set; }

        public int Mode { get; internal set; }

        public int Profile { get; internal set; }

        public int[] UId { get; internal set; }

        public int[] AccelerometerTrims { get; internal set; }

        public string Name { get; internal set; }

        public int NumberofProfiles { get; internal set; }

        public int RateProfile { get; internal set; }

        public int BoardType { get; internal set; }

    }
}
