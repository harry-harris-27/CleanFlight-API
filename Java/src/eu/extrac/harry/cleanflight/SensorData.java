package eu.extrac.harry.cleanflight;

public class SensorData 
{

	private Vector3Float accelerometer;
	private Vector3Float magetometer;
	private Vector3Float gyroscope;
	private Vector3Float kinematics;
	
	
	public SensorData()
	{
		accelerometer = new Vector3Float();
		magetometer = new Vector3Float();
		gyroscope = new Vector3Float();
		kinematics = new Vector3Float();
	}
	
	
	public Vector3Float getAccelerometer() { return accelerometer; }
	public Vector3Float getMagetometer() { return magetometer; }
	public Vector3Float getGyroscope() { return gyroscope; }
	public Vector3Float getKinematics() { return kinematics; }	
}
