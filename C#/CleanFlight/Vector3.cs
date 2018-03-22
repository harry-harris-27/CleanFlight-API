using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    public class Vector3<T>
    {

        public T X { get; set; }
        public T Y { get; set; }
        public T Z { get; set; }


        public Vector3() : this(default(T), default(T), default(T)) { }

        public Vector3(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public static bool operator ==(Vector3<T> lhs, Vector3<T> rhs)
        {
            return lhs.X.Equals(rhs.X) &&
                    lhs.Y.Equals(rhs.Y) &&
                    lhs.Z.Equals(rhs.Z);
        }

        public static bool operator !=(Vector3<T> lhs, Vector3<T> rhs)
        {
            return !(lhs == rhs);
        }



        public override string ToString()
        {
            return "[" + X.ToString() + ", " +
                            Y.ToString() + ", " +
                            Z.ToString() + "]";
        }

    }
}
