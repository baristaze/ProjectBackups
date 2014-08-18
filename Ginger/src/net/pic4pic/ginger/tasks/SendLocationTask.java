package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.os.AsyncTask;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.Location;
import net.pic4pic.ginger.entities.SimpleRequest;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class SendLocationTask extends AsyncTask<String, Void, BaseResponse> {

	private Context context;
	private SendLocationListener listener;
	private Location location; 
	private LocationType locationType;
	
	public SendLocationTask(Context context, SendLocationListener listener, Location location, LocationType locationType) {
		
		this.context = context;
		this.listener = listener;
		this.location = location;
		this.locationType = locationType;
	}
	
	@Override
    protected BaseResponse doInBackground(String... executeArgs) {    	
    	// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to Person
    	try {
    		
    		SimpleRequest<Location> request = new SimpleRequest<Location>();
    		request.setData(this.location);
    		
    		if(this.locationType == LocationType.HintForSupport){    			
    			return Service.getInstance().assureSupportAtLocation(this.context, request);
    		}
    		else{
    			return Service.getInstance().setCurrentLocation(this.context, request);
    		}			
		} 
    	catch (GingerException e) {
    		
    		MyLog.bag().add(e).e();
    		
			BaseResponse response = new BaseResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
    	catch(Exception e){
    		
    		MyLog.bag().add(e).e();
    		
    		BaseResponse response = new BaseResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when ");
			return response; 	
    	}
    }
	
	@Override
	protected void onPostExecute(BaseResponse response) {
		
		if(response != null){
			if(response.getErrorCode() == 0) {
				if(this.listener != null){
					this.listener.onLocationInfoSent(this.location, this.locationType);
				}
			}
			else{
				MyLog.bag().e(response.getErrorMessage());
			}
		}
		else{
			MyLog.bag().e("Returned response is null");
		}
    }
	
	public interface SendLocationListener {
		public void onLocationInfoSent(Location location, LocationType locationType);
	}
	
	public enum LocationType
	{
		HintForSupport,
		CurrentLocation
	}
}
