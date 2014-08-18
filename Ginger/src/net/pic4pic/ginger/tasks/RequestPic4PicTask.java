package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.widget.Button;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.MatchedCandidateResponse;
import net.pic4pic.ginger.entities.StartingPic4PicRequest;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class RequestPic4PicTask extends BlockedTask<String, Void, MatchedCandidateResponse> {
	
	private RequestPic4PicListener listener;
	private StartingPic4PicRequest request;
	
	public RequestPic4PicTask(RequestPic4PicListener listener, Context context, Button button, StartingPic4PicRequest request){			
		super(context, button);		
		this.listener = listener;
		this.request = request;
	}
	
    @Override
    protected MatchedCandidateResponse doInBackground(String... executeArgs) {    	
    	
    	// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to UserResponse
    	try {
			return Service.getInstance().requestPic4Pic(this.context, this.request);
		} 
    	catch (GingerException e) {
    		
    		MyLog.bag().add(e).e();
    		
    		MatchedCandidateResponse response = new MatchedCandidateResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
    	catch(Exception e){
    		
    		MyLog.bag().add(e).e();
    		
    		MatchedCandidateResponse response = new MatchedCandidateResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when sending pic4pic request.");
			return response; 	
    	}
    }

    protected void onPostExecute(MatchedCandidateResponse response) {    	
    	super.onPostExecute(response);    	
    	this.listener.onPic4PicRequestSent(response, this.request);
    }
    
    public interface RequestPic4PicListener{
    	public void onPic4PicRequestSent(MatchedCandidateResponse response, StartingPic4PicRequest request);
    }
}
