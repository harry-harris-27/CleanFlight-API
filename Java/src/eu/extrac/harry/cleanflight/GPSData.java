package eu.extrac.harry.cleanflight;

public class GPSData 
{

	private byte fix;
	private byte numberOfSatellites;
	private double latitude;
	private double longitude;
	private float altitude;
	private float speed;
	private float groundCourse;
	
	
	public GPSData()
	{
		fix = 0;
		numberOfSatellites = 0;
		latitude = 0;
		longitude = 0;
		altitude = 0;
		speed = 0;
		groundCourse = 0;
	}
	
	
	public byte getFix() { return fix; }
	public void setFix(byte fix) { this.fix = fix; }
	
	public byte getNumberOfSatellites() { return numberOfSatellites; }
	public void setNumberOfSatellites(byte numSats) { this.numberOfSatellites = numSats; }
	
	public double getLatitude() { return latitude; }
	public void setLatitude(double latitude) { this.latitude = latitude; }

	public double getLongitude() { return longitude; }
	public void setLongitude(double longitude) { this.longitude = longitude; }
	
	public float getAltitude() { return altitude; }
	public void setAltitude(float altitude) { this.altitude = altitude; }
	
	public float getSpeed() { return speed; }
	public void setSpeed(float speed) { this.speed = speed; }

	public float getGroundCourse() { return groundCourse; }
	public void setGroundCourse(float groundCourse) { this.groundCourse = groundCourse; }
}
