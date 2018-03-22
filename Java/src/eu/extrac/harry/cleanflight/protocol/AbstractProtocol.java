package eu.extrac.harry.cleanflight.protocol;

import java.util.concurrent.ConcurrentLinkedQueue;

import eu.extrac.harry.cleanflight.FlightController;
import eu.extrac.harry.cleanflight.msp.MSPCode;
import eu.extrac.harry.cleanflight.msp.MSPHelper;


public abstract class AbstractProtocol implements Protocol
{
	
	//
	// Private constants
	//
	
	private static final int IN_BUFFER_SIZE = 256;
	private static final byte 	// MSP Process state
		IDLE = 0, 
		HEADER_PREFIX = 1,
		HEADER_M = 2,
		HEADER_ARROW = 3,
		HEADER_SIZE = 4,
		HEADER_CMD = 5,
		HEADER_ERR = 6;
	
	
	//
	// Protected variables
	//
	
	protected FlightController fc;
	
	
	//
	// Private variables
	//
	
	private ConcurrentLinkedQueue<Byte> responseQueue;
	private byte[] inBuffer;
	private byte state = IDLE, checksum = 0;
	private int inOffset = 0, dataSize = 0;
	private boolean errorReceived = false;
	private MSPCode cmd;
	
	
	//
	// Protected Constructors
	//
	
	protected AbstractProtocol()
	{
		fc = null;
		responseQueue = new ConcurrentLinkedQueue<Byte>();
		flush();
	}
	
	
	//
	// Public methods
	//
		
	/**
	 * Closes the connection
	 */
	@Override
	public abstract void close();
	
	/**
	 * Clears any response data being processed
	 */
	public void flush()
	{
		responseQueue.clear();
		
		inBuffer = new byte[IN_BUFFER_SIZE];
		
		inOffset = 0;
		
		state = IDLE;
		checksum = 0;
		dataSize = 0;
		
		errorReceived = false;
		
		cmd = MSPCode.Undefined;
	}
	
	public FlightController getFlightController()
	{
		return fc;
	}
	public void setFlightController(FlightController fc)
	{
		this.fc = fc;
	}
	
	/**
	 * Returns a boolean indicating whether the connection is currently open
	 * @return
	 */
	@Override
	public abstract boolean isOpen();
	
	/**
	 * Opens the connection
	 * @return
	 */
	@Override
	public abstract boolean open();
	
	/**
	 * Send an MSP request command without a payload
	 * @param cmd The MSP command to be sent
	 */
	@Override
	public void sendMSPRequest(MSPCode cmd)
	{
		sendMSPRequest(cmd, null);
	}
	
	/**
	 * Send an MSP request command with a payload
	 * @param cmd The MSP command to be sent
	 * @param payload The request's payload
	 */
	@Override
	public void sendMSPRequest(MSPCode cmd, byte[] payload)
	{
        sendMspRequest(MSPHelper.CreateMSPRequest(cmd, payload));
	}
	
	/**
	 * Sends a prepared MSP request command
	 * @param cmd The prepared MSP request command
	 */
	@Override
	public abstract void sendMspRequest(byte[] cmd);
		
	
	//
	// Protected methods
	//
	
	/**
	 * Enqueues a MSP response to the response queue to be processed
	 * @param response
	 */
	protected void enqueueResponse(byte[] response)
	{
		boolean promptProcess = responseQueue.isEmpty();
		
		for (byte b : response)
		{
			responseQueue.offer(b);
		}
		
		if (promptProcess) processMSPResponse();
	}
		
	
	//
	// Private methods
	//
		
	private void processMSPResponse()
	{				
		// Process all the received bytes
		while (!responseQueue.isEmpty())
		{
			// Get byte to process
			byte b = (byte) responseQueue.poll();
			
			// Header
			if (state == IDLE)
			{
				state = (b == '$') ? HEADER_PREFIX : IDLE;
			}
			
			// 'M'
			else if (state == HEADER_PREFIX)
			{
				state = (b == 'M') ? HEADER_M : IDLE;
			}
			
			// '>' OR '!'
			else if (state == HEADER_M)
			{
				if (b == '>')
				{
					state = HEADER_ARROW;
				}
				else if (b == '!')
				{
					state = HEADER_ERR;
				}
				else
				{
					state = IDLE;
				}
			}
			
			// Payload size
			else if ((state == HEADER_ARROW) || (state == HEADER_ERR))
			{
				// Is it an error message?
				errorReceived = (state == HEADER_ERR);
				
				// Now expecting the payload size
				dataSize = (b & 0xFF);
				
				// Reset index variables
				inOffset = 0;
				checksum = 0;
				checksum ^= (byte)(b & 0xFF);
				
				// Empty the in buffer
				inBuffer = new byte[IN_BUFFER_SIZE];
				
				// The command is to follow
				state = HEADER_SIZE;
			}
			
			// MSP command
			else if (state == HEADER_SIZE)
			{
				cmd = MSPCode.valueOf(b & 0xFF);
				checksum ^= (byte)(b & 0xFF);
				state = HEADER_CMD;
			}
			
			// Payload
			else if ((state == HEADER_CMD) && (inOffset < dataSize))
			{
				checksum ^= (byte)(b & 0xFF);
				inBuffer[inOffset++] = (byte)(b & 0xFF);
			}
			else if ((state == HEADER_CMD) && (inOffset >= dataSize))
			{
				// Compare calculated and received checksum
				if ((checksum & 0xFF) == (b & 0xFF))
				{
					if (errorReceived)
					{
						System.out.println("Device did not understand request type " + cmd.toString());
					}
					else
					{
						if (fc != null) 
						{
							new MSPHelper().evaluateCommand(fc, cmd, inBuffer, 0);
						}
					}
				}
				
				// Reset state
				state = IDLE;
			}
		}
	}
		
}