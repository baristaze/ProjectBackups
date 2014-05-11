package net.pic4pic.ginger.tasks;

import android.os.AsyncTask;

import net.pic4pic.ginger.LaunchActivity;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.UserCredentials;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class SigninTask extends AsyncTask<String, Void, UserResponse> {
	
	private LaunchActivity activity;
	private UserCredentials credentials;
	
	public SigninTask(LaunchActivity activity, UserCredentials credentials) {
		this.activity = activity;
		this.credentials = credentials;
	}

    @Override
    protected UserResponse doInBackground(String... executeArgs) {    	
    	// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to Person
    	try {
			return Service.getInstance().signin(this.activity, this.credentials);
		} 
    	catch (GingerException e) {
    		
    		MyLog.e("Signin", e.toString());
    		
			UserResponse response = new UserResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
    	catch(Exception e){
    		
    		MyLog.e("Signin", e.toString());
    		
    		UserResponse response = new UserResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when ");
			return response; 	
    	}
    }

    protected void onPostExecute(UserResponse result) {
    	this.activity.onSignin(result, this.credentials);
    }
}
