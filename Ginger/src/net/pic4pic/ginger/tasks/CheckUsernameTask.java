package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.widget.Button;

import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.UserCredentials;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.service.Service;

public class CheckUsernameTask extends BlockedTask<String, Void, UserResponse> {
	
	private CheckUsernameListener listener;
	private UserCredentials credentials;
	
	public CheckUsernameTask(CheckUsernameListener listener, Context context, Button button, UserCredentials credentials){			
		super(context, button);		
		this.listener = listener;
		this.credentials = credentials;
	}
		
    @Override
    protected UserResponse doInBackground(String... executeArgs) {    	
    	// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to UserResponse
    	try {
			return Service.getInstance().checkUsername(this.context, this.credentials);
		} 
    	catch (GingerException e) {
			UserResponse response = new UserResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
    	catch(Exception e){
    		UserResponse response = new UserResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Unexpected error when ");
			return response; 	
    	}
    }

    protected void onPostExecute(UserResponse response) {    	
    	super.onPostExecute(response);    	
    	this.listener.onCheckUser(response, this.credentials);
    }
    
    public interface CheckUsernameListener{
    	public void onCheckUser(UserResponse response, UserCredentials credentials);
    }
}
