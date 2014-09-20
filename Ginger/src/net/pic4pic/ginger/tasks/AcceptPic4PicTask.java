package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.widget.Button;

import net.pic4pic.ginger.entities.AcceptingPic4PicRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.MatchedCandidateResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class AcceptPic4PicTask extends BlockedTask<String, Void, MatchedCandidateResponse> {

	private AcceptPic4PicListener listener;
	private AcceptingPic4PicRequest request;
	
	public AcceptPic4PicTask(AcceptPic4PicListener listener, Context context, Button button, AcceptingPic4PicRequest request){			
		super(context, button);		
		this.listener = listener;
		this.request = request;
	}
	
    @Override
    protected MatchedCandidateResponse doInBackground(String... executeArgs) {    	
    	// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to UserResponse
    	try {
			return Service.getInstance().acceptPic4Pic(this.context, this.request);
		} 
    	catch (GingerException e) {
    		
    		MyLog.bag().add(e).e();
    		
    		MatchedCandidateResponse response = new MatchedCandidateResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
    	catch(Throwable e){
    		
    		MyLog.bag().add(e).e();
    		
    		MatchedCandidateResponse response = new MatchedCandidateResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when accepting pic4pic request.");
			return response; 	
    	}
    }

    protected void onPostExecute(MatchedCandidateResponse response) {    	
    	super.onPostExecute(response);    	
    	this.listener.onPic4PicAccepted(response, this.request);
    }
    
    public interface AcceptPic4PicListener{
    	public void onPic4PicAccepted(MatchedCandidateResponse response, AcceptingPic4PicRequest request);
    }
}
