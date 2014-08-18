package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.widget.Button;

import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class ActivateUserTask extends BlockedTask<String, Void, BaseResponse> {

	private ActivateUserListener listener;
	private BaseRequest request;
	
	public ActivateUserTask(ActivateUserListener listener, Context context, Button button, BaseRequest request){			
		super(context, button);		
		this.listener = listener;
		this.request = request;
	}
	
	@Override
    protected BaseResponse doInBackground(String... executeArgs) {    	
    	// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to UserResponse
    	try {
			return Service.getInstance().activateUser(this.context, this.request);
		} 
    	catch (GingerException e) {
    		
    		MyLog.bag().e("ActivateUser", e.toString());
    		
    		BaseResponse response = new BaseResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
    	catch(Exception e){
    		
    		MyLog.bag().e("ActivateUser", e.toString());
    		
    		BaseResponse response = new BaseResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when activating user");
			return response; 	
    	}
    }
	
	protected void onPostExecute(BaseResponse response) {    	
    	super.onPostExecute(response);    	
    	this.listener.onUserActivated(response, this.request);
    }
	
	public interface ActivateUserListener{
    	public void onUserActivated(BaseResponse response, BaseRequest request);
    }
}
