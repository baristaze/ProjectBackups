package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.widget.Button;

public class SignupTask extends BlockedTask<String, Void, String> {
	
	private SignupListener listener;
	private String username;
	private String password;	
	
	public SignupTask(SignupListener listener, Context context, Button button, String username, String password){			
		super(context, button);		
		this.listener = listener;
		this.username = username;
		this.password = password;
	}
		
    @Override
    protected String doInBackground(String... executeArgs) {    	
    	// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to Person
    	try {
			Thread.sleep(1500);
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
    	
    	/*no error message*/
    	return null;
        //return "Sign up failed. Please try again later.";
    }

    protected void onPostExecute(String errorMessage) {    	
    	super.onPostExecute(errorMessage);    	
    	this.listener.onSignup(errorMessage, this.username, this.password);
    }
    
    public interface SignupListener{
    	public void onSignup(String errorMessage, String username, String password);
    }
}
