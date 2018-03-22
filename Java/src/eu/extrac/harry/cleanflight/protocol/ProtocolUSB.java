package eu.extrac.harry.cleanflight.protocol;

import com.fazecast.jSerialComm.*;


public class ProtocolUSB extends AbstractProtocol implements SerialPortDataListener
{

	//
	// Private constants
	//
	
	
	//
	// Private variables
	//
	private SerialPort serialPort;
	private String comPortId;
	private int baudRate;
	
	
	//
	// Constructors
	//
	
	public ProtocolUSB(String id, int baud)
	{
		serialPort = null;
		comPortId = id;
		baudRate = baud;
	}
	
	
	//
	// Public Methods
	//
	
	/**
	 * Closes the connection
	 */
	@Override
	public void close()
	{
		if (isOpen())
		{
			serialPort.closePort();
			serialPort = null;
		}
	}
	
	/**
	 * Returns a boolean indicating whether the connection is currently open
	 * @return
	 */
	@Override
	public boolean isOpen()
	{
		return (serialPort != null) && serialPort.isOpen();
	}
	
	/**
	 * Opens the connection
	 * @return
	 */
	@Override
	public boolean open()
	{
		if (!isOpen())
		{
			serialPort = SerialPort.getCommPort(comPortId);
			serialPort.addDataListener(this);
			serialPort.setBaudRate(baudRate);
			return serialPort.openPort();
		}
		
		return false;
	}
	
	/**
	 * Sends a prepared MSP request command
	 * @param cmd The prepared MSP request command
	 */
	@Override
	public void sendMspRequest(byte[] cmd) 
	{
		if (isOpen())
		{
			serialPort.writeBytes(cmd, cmd.length);
		}
	}


	
	@Override
	public int getListeningEvents() 
	{
		return SerialPort.LISTENING_EVENT_DATA_AVAILABLE;
	}
	


	@Override
	public void serialEvent(SerialPortEvent event) 
	{
		if (event.getEventType() != SerialPort.LISTENING_EVENT_DATA_AVAILABLE)
		{
			return;
		}
	    
		byte[] newData = new byte[serialPort.bytesAvailable()];
	    serialPort.readBytes(newData, newData.length);
	    super.enqueueResponse(newData);
	}
	
	
	//
	// Protected Methods
	//
	
	
	//
	// Private Methods
	//
	
	
}
