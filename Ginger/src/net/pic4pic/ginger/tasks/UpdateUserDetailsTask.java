package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.os.AsyncTask;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.SimpleRequest;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.LogBag;
import net.pic4pic.ginger.utils.MyLog;

public class UpdateUserDetailsTask extends AsyncTask<String, Void, BaseResponse> {
	
	private Context context;
	private UserDetailsUpdateListener listener;
	private SimpleRequest<String> request;
	
	public UpdateUserDetailsTask(Context context, UserDetailsUpdateListener listener, SimpleRequest<String> request){
		this.context = context;
		this.listener = listener;
		this.request = request;
	}
	
	@Override
	protected BaseResponse doInBackground(String... params) {
		
		try {
			
			// make service call
			BaseResponse response = Service.getInstance().updateUserDetails(this.context, this.request);
			
			// log if there is an error
			if(response.getErrorCode() != 0){
				MyLog.bag().add(LogBag.TagMessage, response.getErrorMessage()).e();
			}
			
			// return
			return response;
		} 
		catch (GingerException e) {
    		
			// log
    		MyLog.bag().add(e).e();
    		
    		// return
    		BaseResponse response = new BaseResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
		catch (Exception e) {
			
			// log
			MyLog.bag().add(e).e();
    		
			// return
			BaseResponse response = new BaseResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when updating user's 'instant status'");
			return response; 	
		}	
	}
	
	@Override
	protected void onPostExecute(BaseResponse result) {
		this.listener.onUserDetailsUpdated(result, this.request);
	}
	
	public interface UserDetailsUpdateListener{    	
    	public void onUserDetailsUpdated(BaseResponse response, SimpleRequest<String> request);
    }
}
