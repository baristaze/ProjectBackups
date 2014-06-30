package net.pic4pic.ginger.tasks;

import android.app.Activity;
import android.os.AsyncTask;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.PurchaseRecord;
import net.pic4pic.ginger.entities.PurchaseRecordList;
import net.pic4pic.ginger.entities.SimpleRequest;
import net.pic4pic.ginger.service.InAppPurchasingService;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.PurchaseUtils;

public class PurchaseBackLogTask extends AsyncTask<String, Void, Integer> {

	private Activity activity;
	private PurchaseBackLogListener listener;
	private InAppPurchasingService purchasingService;
	
	public PurchaseBackLogTask(Activity activity, PurchaseBackLogListener listener, InAppPurchasingService purchasingService){
		this.activity = activity;
		this.listener = listener;
		this.purchasingService = purchasingService;
	}
	
	@Override
    protected Integer doInBackground(String... urls) {

		// wait for inApp purchasing service
		
		int allSuccess = 0;		
		int currentCredit = Integer.MIN_VALUE;		
		PurchaseRecordList unprocesseds = PurchaseUtils.readUnprocessedPurchasesFromFile(this.activity);
		for(PurchaseRecord purchase : unprocesseds){			
			int temp = this.safeSendPurchaseRecord(purchase);
			if(temp != Integer.MIN_VALUE){
				currentCredit = temp;
				if(this.safeRemoveUnprocessed(purchase)){
					allSuccess++;
				}
			}
		}
		
		MyLog.i("PurchaseBackLogTask", allSuccess + " of " + unprocesseds.size() + " unprocessed purchases are prosessed successfully.");
		
		allSuccess = 0;
		PurchaseRecordList unconsumeds = PurchaseUtils.readUnconsumedPurchasesFromFile(this.activity);
		for(PurchaseRecord purchase : unconsumeds){
			MyLog.v("PurchaseBackLogTask", "Consuming purchase: " + purchase.getPurchaseReferenceToken());
			int temp = PurchaseUtils.consumePurchaseOnAppStoreAndClearLocal(this.activity, this.purchasingService, purchase);
			if(temp == 2){
				allSuccess++;
			}
		}
		
		MyLog.i("PurchaseBackLogTask", allSuccess + " of " + unconsumeds.size() + " unconsumed purchases are consumed successfully.");
		
		return currentCredit;
	}
	
	@Override
    protected void onPostExecute(Integer currentCredit) {
		if(currentCredit != Integer.MIN_VALUE){
			this.listener.onBackLogProcessingComplete(currentCredit);
		}		
	}
	
	private int safeSendPurchaseRecord(PurchaseRecord purchase){
		
		String token = purchase.getPurchaseReferenceToken();
		
		try {
			// prepare request
			SimpleRequest<PurchaseRecord> request = new SimpleRequest<PurchaseRecord>();
			request.setData(purchase);
			
			// send request
			MyLog.v("PurchaseBackLogTask", "Sending purchase record to the server: " + token);
			BaseResponse response = Service.getInstance().processPurchase(activity, request);
			if(response.getErrorCode() == 0){
				MyLog.i("PurchaseBackLogTask", "Purchase record has been sent to the server successfully. Current Credit: " + response.getCurrentCredit());
				return response.getCurrentCredit();
			}
			else{
				MyLog.e("PurchaseBackLogTask", "Sending purchase record (" + token + ") to the server failed: " + response.getErrorMessage());
				return Integer.MIN_VALUE;
			}
		} 
		catch (GingerException e) {
			MyLog.e("PurchaseBackLogTask", "Sending purchase record (" + token + ") to the server failed: " + e.getMessage());
			return Integer.MIN_VALUE;
		}
		catch(Exception e){
			MyLog.e("PurchaseBackLogTask", "Unknown error when sending purchase record (" + token + ") to the server: " + e.getMessage());
			return Integer.MIN_VALUE;
		}	
	}
	
	private boolean safeRemoveUnprocessed(PurchaseRecord purchase){
		
		String token = purchase.getPurchaseReferenceToken();
		
		try {
			MyLog.v("PurchaseBackLogTask", "Removing unprocessed purchase (" + token + ") from local file: " + purchase.getPurchaseReferenceToken());
			PurchaseUtils.removeUnprocessedPurchaseFromFile(this.activity, purchase);
			MyLog.i("PurchaseBackLogTask", "Removing unprocessed purchase (" + token + ") from local file is successfull: " + purchase.getPurchaseReferenceToken());
			return true;
		} 
		catch (GingerException e) {
			MyLog.e("PurchaseBackLogTask", "Removing purchase record (" + token + ") from local file failed: " + e.getMessage());
		}
		catch(Exception e){
			MyLog.e("PurchaseBackLogTask", "Unknown error when removing purchase record (" + token + ") from local file: " + e.getMessage());
		}		
		
		return false;
	}
	
	public interface PurchaseBackLogListener{
		public void onBackLogProcessingComplete(int currentCredit);
	}
}
