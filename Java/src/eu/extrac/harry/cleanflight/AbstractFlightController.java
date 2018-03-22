package eu.extrac.harry.cleanflight;

import java.util.ArrayList;
import java.util.List;

import eu.extrac.harry.cleanflight.msp.MSPCode;
import eu.extrac.harry.cleanflight.protocol.Protocol;

public abstract class AbstractFlightController implements FlightController
{

	//
	// Private variables
	//
	private Protocol protocol;	
	private List<OnCollectionChangedListener> listeners;
	
	private FlightControllerConfiguration configuration;
	private GPSData gpsData;
	private SensorData sensorData;
	
	
	//
	// Constructors
	//
	
	protected AbstractFlightController(Protocol protocol)
	{
		this.protocol = protocol;
		this.protocol.setFlightController(this);
		
		this.listeners = new ArrayList<OnCollectionChangedListener>();
		
		configuration = new FlightControllerConfiguration();
		gpsData = new GPSData();
		sensorData = new SensorData();
	}
	
	
	//
	// Public methods
	//
	
	public void addOnCollectionChangedListener(OnCollectionChangedListener listener)
	{
		if (listeners.contains(listener))
		{
			return;
		}
		
		listeners.add(listener);
	}
	
	public void onCollectionChanged()
	{
		for (OnCollectionChangedListener listener : listeners)
		{
			if (listener != null)
			{
				listener.onCollectionChanged();
			}
		}
	}
	
	public void close() { protocol.close(); }
	
	public FlightControllerConfiguration getConfiguration() { return configuration; }
	
	public GPSData getGPSData() { return gpsData; }
	
	public SensorData getSensorData() { return sensorData; }
	
	public boolean open() { return protocol.open(); }
	
	public boolean isOpen() { return protocol.isOpen(); }
	
	public void sendMSPRequest(MSPCode cmd) { protocol.sendMSPRequest(cmd); }
	
	public void sendMSPRequest(MSPCode cmd, byte[] payload) { protocol.sendMSPRequest(cmd, payload); }
	
	public void sendMspRequest(byte[] cmd) { protocol.sendMspRequest(cmd); }
	
}
