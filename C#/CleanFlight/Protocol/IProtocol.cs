using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanFlight.Protocol
{
    public interface IProtocol
    {

        //
        // Properties
        //

        /// <summary>
        /// Gets a boolean value indicating whether the protocol is currently open or not
        /// </summary>
        bool IsOpen { get; }
        

        //
        // Methods
        //

        /// <summary>
        /// Opens the connection
        /// </summary>
        /// <returns>A boolean value indicating whether the action of opening the connection was successful</returns>
        bool Open();

        /// <summary>
        /// Closes the connection
        /// </summary>
        void Close();

        /// <summary>
        /// Send an MSP request without a payload
        /// </summary>
        /// <param name="cmd">The MSP command to be sent</param>
        void SendRequestMSP(MSPCode cmd);

        /// <summary>
        /// Send multiple MSP requests without a payload
        /// </summary>
        /// <param name="cmd">The MSP commands to be sent</param>
        void SendRequestMSP(MSPCode[] cmds);

        /// <summary>
        /// Send an MSP request with a payload
        /// </summary>
        /// <param name="cmd">The MSP command to be sent</param>
        /// <param name="payload">The payload of the command</param>
        void SendRequestMSP(MSPCode cmd, byte[] payload);

        /// <summary>
        /// Sends out an already encoded MSP request command
        /// </summary>
        /// <param name="data">The encoded MSP request command</param>
        void SendRequestMSP(byte[] data);

    }
}
