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
		super(activity, button, true, "Saving purchase info...");
		this.activity = activity;
		this.request = request;
		this.listener = listener;	
	}
	
	@Override
	protected BaseResponse doInBackground(String... params) {
		
		try {
			MyLog.bag().v("Task is starting... Making a 'processPurchase' call..");
			BaseResponse response = Service.getInstance().processPurchase(activity, request);
			return response;
		} 
		catch (GingerException e) {
			
			MyLog.bag()
			.add("funnel", "purchase")
			.add("step", "10")
			.add("page", "matches")
			.add("action", "post purchase to server")
			.add("success", "0")
			.add("error", "ginger")
			.m();
			
			// log
			MyLog.bag().add(e).e();
			
			// return
			BaseResponse response = new BaseResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response;
		}
		catch(Throwable e){
			
			MyLog.bag()
			.add("funnel", "purchase")
			.add("step", "10")
			.add("page", "matches")
			.add("action", "post purchase to server")
			.add("success", "0")
			.add("error", "throwable")
			.m();
			
			// log
			String errorMessage = "Unexpected error occurred while processing the purchase";
			MyLog.bag().add(e).e(errorMessage);
			
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
