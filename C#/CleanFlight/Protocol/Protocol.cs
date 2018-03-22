using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight.Protocol
{
    public abstract class Protocol : IProtocol
    {

        //
        // Private constants
        //

        private const string MSP_HEADER = "$M<";
        private const int IN_BUFFER_SIZE = 256;


        //
        // Protected Variables
        //

        protected SynchronizedObservableQueue<byte[]> mspResponseQueue;


        //
        // Constructors
        //

        public Protocol()
        {
            Init();
        }


        //
        // Public Properties
        //

        /// <summary>
        /// Gets a boolean value indicating whether the protocol is currently open or not
        /// </summary>
        public abstract bool IsOpen { get; }


        //
        // Internal Protected Proerties
        //
        
        internal protected FlightController FlightController { get; set; }


        //
        // IProtocol Interface Contract methods
        //


        /// <summary>
        /// Closes the connection
        /// </summary>
        public abstract void Close();
        
        /// <summary>
        /// Opens the connection
        /// </summary>
        /// <returns>A boolean value indicating whether the action of opening the connection was successful</returns>
        public abstract bool Open();

        /// <summary>
        /// Send an MSP request without a payload
        /// </summary>
        /// <param name="cmd">The MSP command to be sent</param>
        public void SendRequestMSP(MSPCode cmd)
        {
            SendRequestMSP(cmd, null);
        }

        /// <summary>
        /// Send multiple MSP requests without a payload
        /// </summary>
        /// <param name="cmd">The MSP commands to be sent</param>
        public void SendRequestMSP(MSPCode[] cmds)
        {
            foreach (MSPCode cmd in cmds)
            {
                SendRequestMSP(cmd, null);
            }
        }

        /// <summary>
        /// Send an MSP request with a payload
        /// </summary>
        /// <param name="cmd">The MSP command to be sent</param>
        /// <param name="payload">The payload of the command</param>
        public void SendRequestMSP(MSPCode cmd, byte[] payload)
        {
            // Is msp cmd valid
            if (cmd == MSPCode.Undefined)
            {
                return;
            }

            // Get bytes from MSP Header string
            List<byte> bf = new List<byte>();
            foreach (char c in MSP_HEADER.ToCharArray())
            {
                bf.Add((byte)c);
            }

            // Add Payload size
            byte payloadSize = (byte)((payload != null ? payload.Length : 0) & 0xFF);
            bf.Add(payloadSize);

            // Add MSP Command
            bf.Add((byte)((byte)cmd & 0xFF));

            // Calculate checksum
            byte checksum = 0;
            checksum ^= (byte)(payloadSize & 0xFF); // Payload Size
            checksum ^= (byte)((byte)cmd & 0xFF);         // MSP Command

            // Add payload to MSP Request and checksum
            if (payload != null)
            {
                foreach (byte b in payload)
                {
                    bf.Add((byte)(b & 0xFF));
                    checksum ^= (byte)(b & 0xFF);
                }
            }

            // Add Checksum to request
            bf.Add(checksum);

            // Return request
            SendRequestMSP(bf.ToArray());
        }

        /// <summary>
        /// Sends out an already encoded MSP request command
        /// </summary>
        /// <param name="data">The encoded MSP request command</param>
        public abstract void SendRequestMSP(byte[] data);

        /// <summary>
        /// Clears the existing response data waiting to be processed
        /// </summary>
        public void Flush()
        {
            lock (mspResponseQueue)
            {
                byte[] outArr;
                while (!mspResponseQueue.IsEmpty)
                {
                    mspResponseQueue.TryDequeue(out outArr);
                }
            }
        }


        //
        // Internal static methods
        //

        static internal int    Read32(byte[] buffer, ref int offset)
        {
            return (buffer[offset++] & 0xff) + ((buffer[offset++] & 0xff) << 8) + ((buffer[offset++] & 0xff) << 16) + ((buffer[offset++] & 0xff) << 24);
        }
        static internal uint   ReadU32(byte[] buffer, ref int offset)
        {
            if (buffer.Length >= offset + 4)
            {
                return (uint)(ReadU16(buffer, ref offset) + (ReadU16(buffer, ref offset) * 65536));
            }

            throw new IndexOutOfRangeException();
        }
        static internal short  Read16(byte[] buffer, ref int offset)
        {
            return (short)((buffer[offset++] & 0xff) + ((buffer[offset++]) << 8));
        }
        static internal ushort ReadU16(byte[] buffer, ref int offset)
        {
            if (buffer.Length >= offset + 2)
            {
                return (ushort)(Read8(buffer, ref offset) + (Read8(buffer, ref offset) * 256));
            }

            throw new IndexOutOfRangeException();
        }
        static internal byte   Read8(byte[] buffer, ref int offset)
        {
            return (byte)(buffer[offset++] & 0xff);
        }


        //
        // Protected methods
        //

        /// <summary>
        /// Given an explict MSP command, the passed buffer is read and required information is extracted 
        /// </summary>
        /// <param name="cmd">The MSP command wo decode the byte data as</param>
        /// <param name="inBuffer">A buffer containing the MSP command response payload</param>
        /// <param name="offset">An offset of for the payload buffer</param>
        protected void EvaluateCommand(MSPCode cmd, byte[] inBuffer, int offset)
        {
            switch (cmd)
            {
                // API Version:
                //   - MSP Protocol Version
                //   - API Version
                case MSPCode.MSP_API_VERSION:
                    FlightController.Configuration.MSPVersion = Read8(inBuffer, ref offset);    // MSP Protocol Version
                    FlightController.Configuration.APIVersion = Read8(inBuffer, ref offset) + "." + Read8(inBuffer, ref offset) + ".0";     // API verison
                    break;

                // Raw IMU:
                //   - Accelerometers [x, y, z]
                //   - Gyroscope [x, y, z]
                case MSPCode.MSP_RAW_IMU:

                    // Scaling assumed at 512
                    FlightController.SensorData.Accelerometer.X = Read16(inBuffer, ref offset) / 512f;
                    FlightController.SensorData.Accelerometer.Y = Read16(inBuffer, ref offset) / 512f;
                    FlightController.SensorData.Accelerometer.Z = Read16(inBuffer, ref offset) / 512f;

                    // Correctly scaled
                    FlightController.SensorData.Gyroscope.X = Read16(inBuffer, ref offset) * (4 / 16.4f);
                    FlightController.SensorData.Gyroscope.Y = Read16(inBuffer, ref offset) * (4 / 16.4f);
                    FlightController.SensorData.Gyroscope.Z = Read16(inBuffer, ref offset) * (4 / 16.4f);

                    break;

                // Raw GPS Data
                //   - Fix
                //   - Number of satallites
                //   - Latitude
                //   - Longitude
                //   - Altitude
                //   - Speed
                //   - Ground course
                case MSPCode.MSP_RAW_GPS:
                    FlightController.GPSData.Fix                = Read8(inBuffer, ref offset);
                    FlightController.GPSData.NumberOfSatellites = Read8(inBuffer, ref offset);
                    FlightController.GPSData.Latitude           = Read32(inBuffer, ref offset) / 10000000d;
                    FlightController.GPSData.Longitude          = Read32(inBuffer, ref offset) / 10000000d;
                    FlightController.GPSData.Altitude           = ReadU16(inBuffer, ref offset);
                    FlightController.GPSData.Speed              = ReadU16(inBuffer, ref offset);
                    FlightController.GPSData.GroundCourse       = ReadU16(inBuffer, ref offset);
                    break;

                case MSPCode.MSP_ATTITUDE:
                    FlightController.SensorData.Kinematics.X = Read16(inBuffer, ref offset) / 10.0f;
                    FlightController.SensorData.Kinematics.Y = Read16(inBuffer, ref offset) / 10.0f;
                    FlightController.SensorData.Kinematics.Z = Read16(inBuffer, ref offset);
                    FlightController.OnKinematicDataChanged(EventArgs.Empty);
                    break;

                default:    // Command evaluation has not been implemented

                    break;
            }
        }


        //
        // Private methods
        //

        private void Init()
        {
            FlightController = null;

            mspResponseQueue = new SynchronizedObservableQueue<byte[]>();
            mspResponseQueue.CollectionChanged += MspResponseQueue_CollectionChanged;
        }

        private void MspResponseQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Proccess the received bytes
            while (!mspResponseQueue.IsEmpty)
            {
                lock (mspResponseQueue)
                {
                    byte[] mspResponse;
                    if (mspResponseQueue.TryDequeue(out mspResponse))
                    {
                        // Define variables
                        byte[] inBuf = new byte[IN_BUFFER_SIZE];
                        MSPState mspState = MSPState.IDLE;
                        bool errorReceived = false;

                        byte checksum = 0;
                        MSPCode cmd = MSPCode.Undefined;
                        int offset = 0, dataSize = 0;

                        // Process each byte in response
                        foreach (byte c in mspResponse)
                        {
                            if (IsOpen)
                            {
                                // Header
                                if (mspState == MSPState.IDLE)
                                {
                                    mspState = (c == '$') ? MSPState.HEADER_START : MSPState.IDLE;
                                }

                                // 'M'
                                else if (mspState == MSPState.HEADER_START)
                                {
                                    mspState = (c == 'M') ? MSPState.HEADER_M : MSPState.IDLE;
                                }

                                // Arrow or '!'
                                else if (mspState == MSPState.HEADER_M)
                                {
                                    if (c == '>')
                                    {
                                        mspState = MSPState.HEADER_ARROW;
                                    }
                                    else if (c == '!')
                                    {
                                        mspState = MSPState.HEADER_ERR;
                                    }
                                    else
                                    {
                                        mspState = MSPState.IDLE;
                                    }
                                }

                                // Payload size
                                else if (mspState == MSPState.HEADER_ARROW || mspState == MSPState.HEADER_ERR)
                                {
                                    // Is this an error message?
                                    errorReceived = (mspState == MSPState.HEADER_ERR);

                                    // Now expecting the payload size
                                    dataSize = (c & 0xFF);

                                    // Reset index variables
                                    offset = 0;
                                    checksum = 0;
                                    checksum ^= (byte)(c & 0xFF);

                                    // The command is to follow
                                    mspState = MSPState.HEADER_SIZE;
                                }

                                // MSP Command
                                else if (mspState == MSPState.HEADER_SIZE)
                                {
                                    cmd = (MSPCode)(byte)(c & 0xFF);
                                    checksum ^= (byte)(c & 0xFF);
                                    mspState = MSPState.HEADER_CMD;
                                }

                                else if (mspState == MSPState.HEADER_CMD && offset < dataSize)
                                {
                                    checksum ^= (byte)(c & 0xFF);
                                    inBuf[offset++] = (byte)(c & 0xFF);
                                }

                                else if (mspState == MSPState.HEADER_CMD && offset >= dataSize)
                                {
                                    // Compare calculated and transferred checksum
                                    if ((checksum & 0xFF) == (c & 0xFF))
                                    {
                                        if (errorReceived)
                                        {
                                            Console.Out.WriteLine("Board did not understand request type " + c);
                                        }
                                        else
                                        {
                                            // We got a valid response packet, evaluate it
                                            EvaluateCommand(cmd, inBuf, 0);
                                        }
                                    }
                                    else
                                    {
                                        Console.Out.WriteLine("invalid checksum for command " + ((byte)cmd & 0xFF) + ": " + (checksum & 0xFF) + " expected, got " + (c & 0xFF));
                                    }

                                    // Ready for anymore responses
                                    mspState = MSPState.IDLE;
                                }
                            }
                        }
                    }
                }
            }
        }
        


        //
        // Protected classes
        //

        /// <summary>
        /// A thread-safe observable queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected class SynchronizedObservableQueue<T> : ConcurrentQueue<T>, INotifyCollectionChanged
        {
            /// <summary>
            /// An event raised when a new item is enqueued to the collection
            /// </summary>
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            /// <summary>
            /// Hides base class enqueue method and calls the base class method while also raising a collection changed event
            /// </summary>
            /// <param name="item">The Data item to add</param>
            public new void Enqueue(T item)
            {
                base.Enqueue(item);

                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

    }
}
