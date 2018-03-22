package eu.extrac.harry.cleanflight.protocol;

import eu.extrac.harry.cleanflight.FlightController;
import eu.extrac.harry.cleanflight.msp.MSPCode;

public interface Protocol 
{

	/**
	 * Returns a boolean indicating whether the connection is currently open
	 * @return
	 */
	boolean isOpen();
	
	/**
	 * Opens the connection
	 * @return
	 */
	boolean open();
	
	/**
	 * Closes the connection
	 */
	void close();
	
	/**
	 * Send an MSP request command without a payload
	 * @param cmd The MSP command to be sent
	 */
	void sendMSPRequest(MSPCode cmd);
	
	/**
	 * Send an MSP request command with a payload
	 * @param cmd The MSP command to be sent
	 * @param payload The request's payload
	 */
	void sendMSPRequest(MSPCode cmd, byte[] payload);
	
	/**
	 * Sends a prepared MSP request command
	 * @param cmd The prepared MSP request command
	 */
	void sendMspRequest(byte[] cmd);	
	
	
	FlightController getFlightController();
	void setFlightController(FlightController fc);
}
