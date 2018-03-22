package eu.extrac.harry.cleanflight;

public class FlightControllerConfiguration 
{

	private String apiVersion;
	private int mspVersion;
	
	
	
	public FlightControllerConfiguration()
	{
		apiVersion = "0.0.0";
		mspVersion = -1;
	}
	
	
	
	public String getAPIVersion()
	{
		return new String(apiVersion);
	}
	public void setAPIVersion(String api)
	{
		apiVersion = new String(api);
	}
	
	public int getMSPVersion()
	{
		return mspVersion;
	}
	public void setMSPVersion(int msp)
	{
		mspVersion = msp;
	}
}
