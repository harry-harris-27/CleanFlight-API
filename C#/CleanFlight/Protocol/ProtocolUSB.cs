using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CleanFlight.Protocol
{
    public class ProtocolUSB : Protocol
    {

        //
        // Private variables
        //

        private SerialPort sPort = null;
        private SerialDataReceivedEventHandler serialRxHandler;


        //
        // Constructors
        //
        
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="port">The serial port to connect to and send and receive data</param>
        /// <param name="baud">The baud rate at which to connect to the serial port</param>
        public ProtocolUSB(string port, int baud) : base() 
        {
            Init();

            PortName = port;
            BaudRate = baud;
        }


        //
        // Public Properties
        //

        /// <summary>
        /// Gets the serial port to connect and send and receive data
        /// </summary>
        public string PortName { get; protected set; }

        /// <summary>
        /// Gets the baud rate at which to connect to the serial port
        /// </summary>
        public int BaudRate { get; protected set; }

        /// <summary>
        /// Gets a boolean value indicating whether the protocol is currently open or not
        /// </summary>
        public override bool IsOpen { get { return (sPort != null) && (sPort.IsOpen); } }


        //
        // Public Methods
        //

        /// <summary>
        /// Closes the connection to the serial port
        /// </summary>
        public override void Close()
        {
            lock (this)
            {
                if (sPort != null && sPort.IsOpen)
                {
                    // Empty the response queue
                    while (!mspResponseQueue.IsEmpty)
                    {
                        byte[] bOut;
                        mspResponseQueue.TryDequeue(out bOut);
                    }

                    sPort.DataReceived -= serialRxHandler;
                    sPort.DiscardInBuffer();
                    sPort.DiscardOutBuffer();

                    Thread.Sleep(500);

                    sPort.Close();
                    sPort.Dispose();

                    sPort = null;
                }
            }
        }

        /// <summary>
        /// Opens the connection to the serial port
        /// </summary>
        /// <returns>A boolean value indicating whether the action of opening the connection was successful</returns>
        public override bool Open()
        {
            lock (this)
            {
                try
                {
                    Close();

                    sPort = new SerialPort(PortName, BaudRate);
                    sPort.DataReceived += serialRxHandler;
                    sPort.Open();

                    return true;
                }
                catch (Exception ex)
                {
                    throw new System.IO.IOException(ex.ToString());
                }                
            }            
        }

        /// <summary>
        /// Sends out an already encoded MSP request command
        /// </summary>
        /// <param name="data">The encoded MSP request command</param>
        public override void SendRequestMSP(byte[] msp)
        {
            sPort.Write(msp, 0, msp.Length); // send the complete byte sequence in one go
        }


        //
        // Protected methods
        //

        /// <summary>
        /// Method to handle raised serial data received events for the serial port
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Arguments assocaiated with the handled event</param>
        protected virtual void SPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Read all bytes from in buffer
            byte[] ba = new byte[sPort.BytesToRead];

            for (int i = 0; i < ba.Length; i++)
            {
                ba[i] = (byte)sPort.ReadByte();
            }

            lock (mspResponseQueue) mspResponseQueue.Enqueue(ba);
        }


        //
        // Private methods
        //

        private void Init()
        {
            sPort = null;
            serialRxHandler = new SerialDataReceivedEventHandler(SPort_DataReceived);

            PortName = "";
            BaudRate = 0;
        }
        
    }
}
