using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using CleanFlight.Protocol;

namespace CleanFlight
{
    public class FlightController : IProtocol
    {

        //
        // Private constants
        //
    
        private const int REFRESH_INTERVAL = 250;


        //
        // Private variables
        //

        private Protocol.Protocol protocol;
        private Timer refreshTimer;
        private MSPCode[] INTERVAL_COMMANDS = new MSPCode[] { MSPCode.MSP_API_VERSION };


        //
        // Constructors
        //

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="protocol">The protocol to connect to the cleanflight board/device</param>
        public FlightController(IProtocol _protocol)
        {
            protocol = (Protocol.Protocol)_protocol;
            protocol.FlightController = this;

            RefreshData = new MSPCode[] 
            {
                MSPCode.MSP_RAW_IMU,
                MSPCode.MSP_RAW_GPS,
                MSPCode.MSP_ATTITUDE
            };

            Configuration = new FlightControllerConfiguration();
            FeatureConfiguration = new FeatureConfiguration();
            MixerConfiguration = new MixerConfiguration();
            BoardAlignmentConfiguration = new BoardAlignmentConfiguration();
            RC = new RC();
            RCTuning = new RCTuning();
            GPSData = new GPSData();
            SensorData = new SensorData();

            refreshTimer = new Timer(REFRESH_INTERVAL);
            refreshTimer.Elapsed += RefreshTimer_Elapsed;
        }


        //
        // Events
        //

        /// <summary>
        /// An event raised when kinematic data has been changed
        /// </summary>
        public event EventHandler KinematicDataAvailable;
        

        //
        // Public Properties
        //

        /// <summary>
        /// Gets or sets the protocol to connect to the cleanflight board/device
        /// </summary>
        public Protocol.Protocol Protocol
        {
            get
            {
                return protocol;
            }
            set
            {
                protocol.FlightController = this;
                protocol = value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the protocol is currently open or not
        /// </summary>
        public bool IsOpen { get { return (Protocol != null) && (Protocol.IsOpen); } }

        /// <summary>
        /// Gets or sets the commands to be regularly refreshed
        /// </summary>
        public MSPCode[] RefreshData { get; set; }

        /// <summary>
        /// Gets or sets the interval, expressed in milliseconds, at which to refresh the Flight Controller data
        /// </summary>
        public double RefreshInterval
        {
            get
            {
                return refreshTimer.Interval;
            }
            set
            {
                refreshTimer.Interval = value;
            }
        }

        public FlightControllerConfiguration Configuration { get; internal set; }

        public FeatureConfiguration FeatureConfiguration { get; internal set; }

        public MixerConfiguration MixerConfiguration { get; internal set; }

        public BoardAlignmentConfiguration BoardAlignmentConfiguration { get; internal set; }

        public RC RC { get; internal set; }

        public RCTuning RCTuning { get; internal set; }

        public GPSData GPSData { get; internal set; }

        public SensorData SensorData { get; internal set; }


        //
        // Public Methods
        //

        /// <summary>
        /// Opens the protocol connection
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (Protocol != null)
            {
                bool open = Protocol.Open();
                Protocol.SendRequestMSP(MSPCode.MSP_API_VERSION);

                refreshTimer.Start();

                return open;
            }

            return false;
        }

        /// <summary>
        /// Closes the protocol connection
        /// </summary>
        public void Close()
        {
            if (Protocol != null && Protocol.IsOpen)
            {
                refreshTimer.Stop();
                Protocol.Close();
            }
        }

        /// <summary>
        /// Send an MSP request without a payload
        /// </summary>
        /// <param name="cmd">The MSP command to be sent</param>
        void IProtocol.SendRequestMSP(MSPCode cmd)
        {
            if (Protocol != null)
            {
                Protocol.SendRequestMSP(cmd);
            }
        }

        /// <summary>
        /// Send multiple MSP requests without a payload
        /// </summary>
        /// <param name="cmd">The MSP commands to be sent</param>
        void IProtocol.SendRequestMSP(MSPCode[] cmds)
        {
            if (Protocol != null)
            {
                Protocol.SendRequestMSP(cmds);
            }
        }

        /// <summary>
        /// Send an MSP request with a payload
        /// </summary>
        /// <param name="cmd">The MSP command to be sent</param>
        /// <param name="payload">The payload of the command</param>
        void IProtocol.SendRequestMSP(MSPCode cmd, byte[] payload)
        {
            if (Protocol != null)
            {
                Protocol.SendRequestMSP(cmd, payload);
            }
        }

        /// <summary>
        /// Sends out an already encoded MSP request command
        /// </summary>
        /// <param name="data">The encoded MSP request command</param>
        void IProtocol.SendRequestMSP(byte[] data)
        {
            if (Protocol != null)
            {
                Protocol.SendRequestMSP(data);
            }
        }
            
        
        //
        // Internal methods
        //

        public void OnKinematicDataChanged(EventArgs e)
        {
            KinematicDataAvailable?.Invoke(this, e);
        }


        //
        // Private methods
        //

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsOpen)
            {
                Protocol.SendRequestMSP(RefreshData);
            }
        }

    }
}
