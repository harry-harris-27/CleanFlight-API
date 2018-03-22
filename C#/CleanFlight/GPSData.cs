using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    public class GPSData
    {

        public GPSData()
        {
            Fix = 0;
            NumberOfSatellites = 0;
            Latitude = 0;
            Longitude = 0;
            Altitude = 0;
            Speed = 0;
            GroundCourse = 0;
        }
        

        public int Fix { get; internal set; }

        public int NumberOfSatellites { get; internal set; }

        /// <summary>
        /// The Latitude expressed in degrees
        /// </summary>
        public double Latitude { get; internal set; }
        
        /// <summary>
        /// The Longitude expressed in degrees
        /// </summary>
        public double Longitude { get; internal set; }

        /// <summary>
        /// The Altitude expressed in meters
        /// </summary>
        public ushort Altitude { get; internal set; }

        /// <summary>
        /// The speed expressed in cm/s
        /// </summary>
        public ushort Speed { get; internal set; }

        /// <summary>
        /// The Ground Course expressed in degrees
        /// </summary>
        public ushort GroundCourse { get; internal set; }


        public override string ToString()
        {
            object[] print = new object[]
            {
                Fix,
                NumberOfSatellites,
                Latitude,
                Longitude,
                Altitude,
                Speed,
                GroundCourse
            };

            string s = "[";
            for (int i = 0; i < print.Length; i++) s += print[i] + (i == print.Length - 1 ? "]" : ", ");

            return s;
        }


    }
}
