package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.widget.Button;

import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.entities.VerifyBioRequest;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class VerifyBioTask extends BlockedTask<String, Void, UserResponse> {
	
	private VerifyBioListener listener;
	private VerifyBioRequest request;
	private int matchingTextViewId;
	
	public VerifyBioTask(VerifyBioListener listener, Context context, Button button, VerifyBioRequest request, int matchingTextViewId){			
		super(context, button);		
		this.listener = listener;
		this.request = request;
		this.matchingTextViewId = matchingTextViewId;
	}
		
    @Override
    protected UserResponse doInBackground(String... executeArgs) {    	
    	// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to UserResponse
    	try {
			return Service.getInstance().verifyBio(this.context, this.request);
		} 
    	catch (GingerException e) {
    		
    		MyLog.bag().e("VerifyBio", e.toString());
    		
			UserResponse response = new UserResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
    	catch(Exception e){
    		
    		MyLog.bag().e("VerifyBio", e.toString());
    		
    		UserResponse response = new UserResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when verifying your bio");
			return response; 	
    	}
    }

    protected void onPostExecute(UserResponse response) {    	
    	super.onPostExecute(response);    	
    	this.listener.onVerifyBio(response, this.request, this.matchingTextViewId);
    }
    
    public interface VerifyBioListener{
    	public void onVerifyBio(UserResponse response, VerifyBioRequest request, int matchingTextViewId);
    }
}
