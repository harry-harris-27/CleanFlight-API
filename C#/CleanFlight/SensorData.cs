using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    public class SensorData
    {

        public SensorData()
        {
            Gyroscope = new Vector3<float>();
            Accelerometer = new Vector3<float>();
            Magetometer = new Vector3<float>();
            Altitide = 0;
            Sonar = 0;
            Kinematics = new Vector3<float>();
        }


        public Vector3<float> Gyroscope { get; internal set; }

        public Vector3<float> Accelerometer { get; internal set; }

        public Vector3<float> Magetometer { get; internal set; }

        public float Altitide { get; internal set; }

        public float Sonar { get; internal set; }

        /// <summary>
        /// X => Angle X (degrees)
        /// Y => Angle Y (degrees)
        /// Z => Heading (degrees)
        /// </summary>
        public Vector3<float> Kinematics { get; internal set; }

    }
}
