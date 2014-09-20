package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.widget.Button;

import net.pic4pic.ginger.entities.BuyingNewMatchRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.MatchedCandidateListResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class BuyNewMatchTask extends BlockedTask<String, Void, MatchedCandidateListResponse> {

	private Context activity;
	private BuyingNewMatchRequest request;
	private BuyNewMatchListener listener;
	
    public BuyNewMatchTask(Context activity, BuyNewMatchListener listener, BuyingNewMatchRequest request, Button button) {
    	super(activity, button, true, "Retrieving new matches...");
		this.activity = activity;
		this.request = request;
		this.listener = listener;
	}
	
	@Override
    protected MatchedCandidateListResponse doInBackground(String... executeArgs) {
		
		// make an HTTP post in a RESTfull way. Use JSON. 
    	// Once you get the data, convert it to Person
    	try {
    		return Service.getInstance().buyNewMatches(this.activity, this.request);
		} 
    	catch (GingerException e) {
    		
    		MyLog.bag().add(e).e();
    		
    		MatchedCandidateListResponse response = new MatchedCandidateListResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response; 
		}
    	catch(Throwable e){
    		
    		MyLog.bag().add(e).e();
    		
    		MatchedCandidateListResponse response = new MatchedCandidateListResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when retrieving new matches");
			return response; 	
    	}
    }
    
	protected void onPostExecute(MatchedCandidateListResponse result) {
		super.onPostExecute(result);
    	this.listener.onBuyNewMatchComplete(result, this.request);
    }
	
    public interface BuyNewMatchListener{    	
    	public void onBuyNewMatchComplete(MatchedCandidateListResponse response, BuyingNewMatchRequest request);
    }
}
