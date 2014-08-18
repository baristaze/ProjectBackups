package net.pic4pic.ginger.tasks;

import java.util.ArrayList;

import android.app.Activity;
import android.os.AsyncTask;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.InAppPurchaseResult;
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
	
	private ArrayList<InAppPurchaseResult> ghostPurchases = new ArrayList<InAppPurchaseResult>();
	
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
		
		MyLog.bag().i(allSuccess + " of " + unprocesseds.size() + " unprocessed purchases are prosessed successfully.");
		
		allSuccess = 0;
		PurchaseRecordList unconsumeds = PurchaseUtils.readUnconsumedPurchasesFromFile(this.activity);
		for(PurchaseRecord purchase : unconsumeds){
			MyLog.bag().v("Consuming purchase: " + purchase.getPurchaseReferenceToken());
			int temp = PurchaseUtils.consumePurchaseOnAppStoreAndClearLocal(this.activity, this.purchasingService, purchase);
			if(temp == 2){
				allSuccess++;
			}
		}
		
		MyLog.bag().i(allSuccess + " of " + unconsumeds.size() + " unconsumed purchases are consumed successfully.");
		
		if(allSuccess == unconsumeds.size()){			
			this.ghostPurchases = this.safeRetrieveGhostPurchases();
		}		
		
		return currentCredit;
	}
	
	@Override
    protected void onPostExecute(Integer currentCredit) {
		this.listener.onBackLogProcessingComplete(currentCredit, this.ghostPurchases);		
	}
	
	private ArrayList<InAppPurchaseResult> safeRetrieveGhostPurchases(){
		
		try {
			MyLog.bag().v("Retrieving ghost purchases...");
			ArrayList<InAppPurchaseResult> ownedItems = this.purchasingService.getPurchasedItems();
			MyLog.bag().i("Owned Item Count: " + ownedItems.size());
			return ownedItems;
		} 
		catch (GingerException e) {
			e.printStackTrace();
			MyLog.bag().e("Retrieving ghost purchases failed: " + e.getMessage());
		}
		catch(Exception e){
			e.printStackTrace();
			MyLog.bag().e("Unexpected error when retrieving ghost purchases: " + e.getMessage());
		}
		
		return new ArrayList<InAppPurchaseResult>();
	}
	
	private int safeSendPurchaseRecord(PurchaseRecord purchase){
		
		String token = purchase.getPurchaseReferenceToken();
		
		try {
			// prepare request
			SimpleRequest<PurchaseRecord> request = new SimpleRequest<PurchaseRecord>();
			request.setData(purchase);
			
			// send request
			MyLog.bag().v("Sending purchase record to the server: " + token);
			BaseResponse response = Service.getInstance().processPurchase(activity, request);
			if(response.getErrorCode() == 0){
				MyLog.bag().i("Purchase record has been sent to the server successfully. Current Credit: " + response.getCurrentCredit());
				return response.getCurrentCredit();
			}
			else{
				MyLog.bag().e("Sending purchase record (" + token + ") to the server failed: " + response.getErrorMessage());
				return Integer.MIN_VALUE;
			}
		} 
		catch (GingerException e) {
			MyLog.bag().add(e).e("Sending purchase record (" + token + ") to the server failed");
			return Integer.MIN_VALUE;
		}
		catch(Exception e){
			MyLog.bag().add(e).e("Unknown error when sending purchase record (" + token + ") to the server");
			return Integer.MIN_VALUE;
		}	
	}
	
	private boolean safeRemoveUnprocessed(PurchaseRecord purchase){
		
		String token = purchase.getPurchaseReferenceToken();
		
		try {
			MyLog.bag().v("Removing unprocessed purchase (" + token + ") from local file: " + purchase.getPurchaseReferenceToken());
			PurchaseUtils.removeUnprocessedPurchaseFromFile(this.activity, purchase);
			MyLog.bag().i("Removing unprocessed purchase (" + token + ") from local file is successfull: " + purchase.getPurchaseReferenceToken());
			return true;
		} 
		catch (GingerException e) {
			MyLog.bag().add(e).e("Removing purchase record (" + token + ") from local file failed");
		}
		catch(Exception e){
			MyLog.bag().add(e).e("Unknown error when removing purchase record (" + token + ") from local file");
		}		
		
		return false;
	}
	
	public interface PurchaseBackLogListener{
		public void onBackLogProcessingComplete(int currentCredit, ArrayList<InAppPurchaseResult> ghostPurchases);
	}
}
