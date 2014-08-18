package net.pic4pic.ginger.utils;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;

import net.pic4pic.ginger.R;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.PurchaseRecord;
import net.pic4pic.ginger.entities.PurchaseRecordList;
import net.pic4pic.ginger.service.InAppPurchasingService;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.NonBlockedTask;

public class PurchaseUtils {

	public static PurchaseRecordList readUnprocessedPurchasesFromFile(Activity activity){
		return readPurchaseFromFile(activity, R.string.purchase_file_unprocessed_purchases_key);
	}
	
	public static PurchaseRecordList readUnconsumedPurchasesFromFile(Activity activity){
		return readPurchaseFromFile(activity, R.string.purchase_file_unconsumed_purchases_key);
	}
	
	protected static PurchaseRecordList readPurchaseFromFile(Activity activity, int keyId){
		
		SharedPreferences purchasesFile = activity.getSharedPreferences(
				activity.getString(R.string.purchase_filename_key), Context.MODE_PRIVATE);
		
		String key = activity.getString(keyId);
		
		String purchasesAsJsonText = purchasesFile.getString(key, "");
		if(purchasesAsJsonText == null || purchasesAsJsonText.trim().length() == 0){
			return new PurchaseRecordList();
		}
		
		PurchaseRecordList purchases = GingerNetUtils.createFromJsonString(purchasesAsJsonText, PurchaseRecordList.class);
		if(purchases == null){
			return new PurchaseRecordList();
		}
		
		return purchases;
	}
	
	public static PurchaseRecordList saveUnprocessedPurchaseToFile(Activity activity, PurchaseRecord purchaseRecord) throws GingerException {
		
		return savePurchaseToFile(activity, purchaseRecord, R.string.purchase_file_unprocessed_purchases_key);
	}
	
	public static PurchaseRecordList saveUnconsumedPurchaseToFile(Activity activity, PurchaseRecord purchaseRecord) throws GingerException {
		
		return savePurchaseToFile(activity, purchaseRecord, R.string.purchase_file_unconsumed_purchases_key);
	}
	
	protected static PurchaseRecordList savePurchaseToFile(Activity activity, PurchaseRecord purchaseRecord, int keyId) throws GingerException {
		
		SharedPreferences purchasesFile = activity.getSharedPreferences(
				activity.getString(R.string.purchase_filename_key), Context.MODE_PRIVATE);
		
		String key = activity.getString(keyId);
		
		PurchaseRecordList purchases = null;
		String purchasesAsJsonText = purchasesFile.getString(key, "");
		if(purchasesAsJsonText != null && purchasesAsJsonText.length() > 0){
			purchases = GingerNetUtils.createFromJsonString(purchasesAsJsonText, PurchaseRecordList.class);
		}
		
		if(purchases == null){
			purchases = new PurchaseRecordList();
		}
		
		purchases.add(purchaseRecord);
		
		String updatedPurchasesJSON = GingerNetUtils.convertToJsonString(purchases);		
		SharedPreferences.Editor editor = purchasesFile.edit();		
		editor.putString(key, updatedPurchasesJSON);		
		editor.commit();
		
		return purchases;
	}
	
	public static PurchaseRecordList removeUnprocessedPurchaseFromFile(Activity activity, PurchaseRecord purchaseRecord) throws GingerException {
		return removePurchaseFromFile(activity, purchaseRecord, R.string.purchase_file_unprocessed_purchases_key, "unprocessed");
	}
	
	public static PurchaseRecordList removeUnconsumedPurchaseFromFile(Activity activity, PurchaseRecord purchaseRecord) throws GingerException {
		return removePurchaseFromFile(activity, purchaseRecord, R.string.purchase_file_unconsumed_purchases_key, "unconsumed");
	}
	
	protected static PurchaseRecordList removePurchaseFromFile(Activity activity, PurchaseRecord purchaseRecord, int keyId, String type) throws GingerException {
		
		SharedPreferences purchasesFile = activity.getSharedPreferences(
				activity.getString(R.string.purchase_filename_key), Context.MODE_PRIVATE);
		
		String key = activity.getString(keyId);
		
		String purchasesAsJsonText = purchasesFile.getString(key, "");
		if(purchasesAsJsonText == null || purchasesAsJsonText.trim().length() == 0){
			MyLog.bag().w("PurchaseUtils", "There is not any saved " + type + " purchase to remove.");
			return new PurchaseRecordList();
		}
		
		PurchaseRecordList purchases = GingerNetUtils.createFromJsonString(purchasesAsJsonText, PurchaseRecordList.class);
		if(purchases == null || purchases.size() == 0){
			MyLog.bag().w("PurchaseUtils", "There is not any saved " + type + " purchase to remove.");
			return new PurchaseRecordList();
		}
		
		boolean found = false;
		for(PurchaseRecord existing : purchases){
			if(existing.getPurchaseReferenceToken().equals(purchaseRecord.getPurchaseReferenceToken())){
				MyLog.bag().i("PurchaseUtils", "The " + type + " purchase has been found and will be removed: " + purchaseRecord.getPurchaseReferenceToken());
				purchases.remove(existing);
				found = true;
				break;
			}
		}
		
		if(!found){
			MyLog.bag().w("PurchaseUtils", "The " + type + " purchase with token = " + purchaseRecord.getPurchaseReferenceToken() + " couldn't be found to remove");
			return new PurchaseRecordList();
		}
		
		String updatedPurchasesJSON = GingerNetUtils.convertToJsonString(purchases);		
		SharedPreferences.Editor editor = purchasesFile.edit();		
		editor.putString(key, updatedPurchasesJSON);		
		editor.commit();
		
		return purchases;
	}

	public static void startConsumingPurchaseOnAppStoreAndClearLocal(final Activity activity, final InAppPurchasingService purchasingService, final PurchaseRecord purchase){
		
		NonBlockedTask.SafeRun(new ITask(){
			@Override
			public void perform() {				
				consumePurchaseOnAppStoreAndClearLocal(activity, purchasingService, purchase);	
			}
		});
	}
	
	public static int consumePurchaseOnAppStoreAndClearLocal(final Activity activity, final InAppPurchasingService purchasingService, final PurchaseRecord purchase){
	
		boolean consumed = false;
		try {
			purchasingService.consumePurchaseToEnableReBuy(purchase.getPurchaseReferenceToken());
			consumed = true;
		} 
		catch (GingerException e) {
			MyLog.bag().e("PurchaseUtils", "Consuming purchase is failed for the purchase token: " + purchase.getPurchaseReferenceToken());
		}
		
		// update the file
		if(consumed) {
			try {
				MyLog.bag().v("PurchaseUtils", "Removing the unconsumed purchase record from file: " + purchase.getPurchaseReferenceToken());
				PurchaseUtils.removeUnconsumedPurchaseFromFile(activity, purchase);
				return 2;
			} 
			catch (GingerException e) {
				MyLog.bag().e("PurchaseUtils", "Removing the unconsumed purchase record from file failed: " + e.toString());
				return 1;
			}
		}
		else{
			return -1;
		}
	}
}
