package net.pic4pic.ginger.tasks;

import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.NotificationListResponse;
import net.pic4pic.ginger.entities.NotificationRequest;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;
import android.content.Context;
import android.os.AsyncTask;

public class NotificationsTask extends AsyncTask<String, Void, NotificationListResponse> {

	private Context activity;
	private NotificationRequest request;
	private NotificationsListener listener;
	
	public NotificationsTask(Context activity, NotificationsListener listener, NotificationRequest request) {
		this.activity = activity;
		this.request = request;
		this.listener = listener;
	}
	
	@Override
    protected NotificationListResponse doInBackground(String... executeArgs) {
		
		// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to Person
    	try {
    		return Service.getInstance().getNotifications(this.activity, this.request);
		} 
    	catch (GingerException e) {
    		
    		MyLog.bag().e("Notifications", e.toString());
    		
    		NotificationListResponse response = new NotificationListResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
    	catch(Exception e){
    		
    		MyLog.bag().e("Notifications", e.toString());
    		
    		NotificationListResponse response = new NotificationListResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when retrieving notifications");
			return response; 	
    	}
    }

    protected void onPostExecute(NotificationListResponse result) {
    	this.listener.onNotificationsComplete(result, this.request);
    }
    
    public interface NotificationsListener{    	
    	public void onNotificationsComplete(NotificationListResponse response, NotificationRequest request);
    }
}
