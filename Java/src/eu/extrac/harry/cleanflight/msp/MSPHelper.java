package eu.extrac.harry.cleanflight.msp;

import java.util.ArrayList;
import java.util.List;

import eu.extrac.harry.cleanflight.FlightController;

public final class MSPHelper 
{

	//
	// Public Constants
	//
	
	public static final String MSP_REQUEST_HEADER = "$M<";
	//public static final String MSP_RESPONSE_HEADER = "$M>";
	
	
	//
	// Private variable
	//
	private byte[] inBuffer;
	private int offset;
	
	
	//
	// Public methods
	//
	
	public static final byte[] CreateMSPRequest(MSPCode cmd)
	{
		return CreateMSPRequest(cmd, null);
	}
	
	public static final byte[] CreateMSPRequest(MSPCode cmd, byte[] payload)
	{
		// If undefined MSP Code, do not process
		if (cmd == MSPCode.Undefined)
		{
			return null;
		}
		
		// Define variables
		List<Byte> lBytes = new ArrayList<Byte>();
		byte payloadSize = 0;
		byte checksum = 0;
		
		// Get Request header bytes
		for (byte b : MSP_REQUEST_HEADER.getBytes())
		{
			lBytes.add(b);
		}
		
		// Calculate payload and add to byte list
		payloadSize = (byte) ((payload != null ? (int) (payload.length) : 0) & 0xFF);
		lBytes.add(payloadSize);
		
		// Add MSP commmand
		lBytes.add((byte) cmd.value());
		
		// Calculate checksum
        checksum ^= (byte)(payloadSize & 0xFF); // Payload Size
        checksum ^= (byte) cmd.value();         // MSP Command
        
        // Add payload to MSP request and checksum
        if (payload != null)
        {
            for (byte b : payload)
            {
                lBytes.add(b);
                checksum ^= b;
            }
        }
        
        // Add checksum to MSP request
        lBytes.add(checksum);
        
        // Return the byte array
        return toPrimitiveBytes(lBytes.toArray(new Byte[lBytes.size()]));
	}

	public final boolean evaluateCommand(FlightController fc, MSPCode cmd, byte[] payload, int _offset)
	{
		// Reset variables
		inBuffer = payload;
		offset = _offset;
		
		// Process MSP response
		switch (cmd)
		{
			// API Version:
			//	 - MSP Protocol Version
			//	 - API Version
			case MSP_API_VERSION:
				fc.getConfiguration().setMSPVersion(read8());
				fc.getConfiguration().setAPIVersion(read8() + "." + read8() + ".0");
				break;
				
			// Raw IMU:
            //   - Accelerometers [x, y, z]
            //   - Gyroscope [x, y, z]
            case MSP_RAW_IMU:

                // Scaling assumed at 512
                fc.getSensorData().getAccelerometer().x = read16() / 512f;
                fc.getSensorData().getAccelerometer().y = read16() / 512f;
                fc.getSensorData().getAccelerometer().z = read16() / 512f;

                // Correctly scaled
                fc.getSensorData().getGyroscope().x = read16() * (4 / 16.4f);
                fc.getSensorData().getGyroscope().y = read16() * (4 / 16.4f);
                fc.getSensorData().getGyroscope().z = read16() * (4 / 16.4f);

                // No clue about scaling factor
                fc.getSensorData().getMagetometer().x = read16() / 1090f;
                fc.getSensorData().getMagetometer().y = read16() / 1090f;
                fc.getSensorData().getMagetometer().z = read16() / 1090f;

                break;

            // Raw GPS Data
            //   - Fix
            //   - Number of satallites
            //   - Latitude
            //   - Longitude
            //   - Altitude
            //   - Speed
            //   - Ground course
            case MSP_RAW_GPS:
                fc.getGPSData().setFix(read8());
                fc.getGPSData().setNumberOfSatellites(read8());
                fc.getGPSData().setLatitude(read32() / 10000000);
                fc.getGPSData().setLongitude(read32() / 10000000);
                fc.getGPSData().setAltitude(readU16());
                fc.getGPSData().setSpeed(readU16());
                fc.getGPSData().setGroundCourse(readU16());
                break;

            case MSP_ATTITUDE:
                fc.getSensorData().getKinematics().x = read16() / 10.0f;
                fc.getSensorData().getKinematics().y = read16() / 10.0f;
                fc.getSensorData().getKinematics().z = read16();
                break;
				
			default:	// Command evaluation has not been implemented just yet
				return false;
		}
		
		
		fc.onCollectionChanged();
		return true;
	}
	
	
	//
	// Private methods
	//	
	
	private static final byte[] toPrimitiveBytes(Byte[] oBytes)
	{
	    byte[] bytes = new byte[oBytes.length];

	    for(int i = 0; i < oBytes.length; i++) {
	        bytes[i] = oBytes[i];
	    }

	    return bytes;
	}

	private final long readU32()
	{
		if (inBuffer.length >= offset + 4)
		{
			return (long)((readU16() << 0xFFFF) | readU16());
		}
		
		throw new IllegalArgumentException("");
	}
	
	private final int read32()
	{
		return (int)(readU32() & 0xFFFFFFFF);
	}
	
	private final int readU16()
	{
		if (inBuffer.length >= offset + 2)
		{
			return (int)(read8() << 8 | read8());
		}
		
		throw new IllegalArgumentException("");
	}
	
	private final short read16()
	{
		return (short)(readU16() & 0xFFFF);
	}
	
	private final byte read8()
	{
		return (byte)(inBuffer[offset++] & 0xFF);
	}
		
	
}
