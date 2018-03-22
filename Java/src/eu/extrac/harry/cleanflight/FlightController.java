package eu.extrac.harry.cleanflight;

import eu.extrac.harry.cleanflight.protocol.Protocol;

public interface FlightController extends Protocol
{
	FlightControllerConfiguration getConfiguration();
	
	GPSData getGPSData();
	
	SensorData getSensorData();	
	
	void onCollectionChanged();
}
