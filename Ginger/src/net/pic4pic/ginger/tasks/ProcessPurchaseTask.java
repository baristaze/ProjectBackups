package net.pic4pic.ginger.tasks;

import android.content.Context;
import android.widget.Button;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.PurchaseRecord;
import net.pic4pic.ginger.entities.SimpleRequest;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class ProcessPurchaseTask extends BlockedTask<String, Void, BaseResponse> {

	private Context activity;
	private SimpleRequest<PurchaseRecord> request;
	private PurchaseProcessListener listener;
	
	public ProcessPurchaseTask(Context activity, PurchaseProcessListener listener, SimpleRequest<PurchaseRecord> request, Button button){
		super(activity, button, true, "Processing purchase...");
		this.activity = activity;
		this.request = request;
		this.listener = listener;	
	}
	
	@Override
	protected BaseResponse doInBackground(String... params) {
		
		try {
			BaseResponse response = Service.getInstance().processPurchase(activity, request);
			return response;
		} 
		catch (GingerException e) {
			
			// log
			MyLog.e("ProcessPurchaseTask", e.getMessage());
			
			// return
			BaseResponse response = new BaseResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response;
		}
		catch(Exception e){
			
			// log
			String errorMessage = "Unexpected error occurred while processing the purchase";
			MyLog.e("ProcessPurchaseTask", errorMessage + ": " + e.getMessage());
			
			// return
			BaseResponse response = new BaseResponse();
			response.setErrorCode(1);
			response.setErrorMessage(errorMessage);
			return response;
		}
	}
	
	protected void onPostExecute(BaseResponse response) {    	
    	super.onPostExecute(response);    	
    	this.listener.onPurchaseProcessed(response, this.request);
    }
	
	public interface PurchaseProcessListener{
		public void onPurchaseProcessed(BaseResponse response, SimpleRequest<PurchaseRecord> request);
	}
}
