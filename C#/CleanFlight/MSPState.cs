using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight
{
    internal enum MSPState
    {
        IDLE = 0,
        HEADER_START = 1,
        HEADER_M = 2,
        HEADER_ARROW = 3,
        HEADER_SIZE = 4,
        HEADER_CMD = 5,
        HEADER_ERR = 6
    }
}
