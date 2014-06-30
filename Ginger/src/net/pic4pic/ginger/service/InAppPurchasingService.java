package net.pic4pic.ginger.service;

import java.util.ArrayList;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.IntentSender.SendIntentException;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.IBinder;
import android.os.RemoteException;

import com.android.vending.billing.IInAppBillingService;

import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.InAppPurchaseResult;
import net.pic4pic.ginger.entities.InAppPurchaseState;
import net.pic4pic.ginger.utils.MyLog;

public class InAppPurchasingService {
	
	public interface PurchasingServiceListener{
		public void onPurchasingServiceConnected();
	}
	
	private Activity parent;
	private IInAppBillingService billingService;
	private ServiceConnection serviceConnection;
	private PurchasingServiceListener listener;
	
	public static final int INAPP_PURCHASE_REQUEST_CODE = 15001;
	
	private static final int BILLING_API_VERSION = 3; // Google Play
	
	private static final String INAPP_PURCHASE = "inapp";
	// private static final String SUBSCRIPTION = "subs";
	
	private static final int BILLING_RESPONSE_RESULT_OK = 0;					// Success
	private static final int BILLING_RESPONSE_RESULT_USER_CANCELED = 1;			// User pressed back or canceled a dialog
	private static final int BILLING_RESPONSE_RESULT_BILLING_UNAVAILABLE = 3;	// Billing API version is not supported for the type requested
	private static final int BILLING_RESPONSE_RESULT_ITEM_UNAVAILABLE = 4;		// Requested product is not available for purchase
	private static final int BILLING_RESPONSE_RESULT_DEVELOPER_ERROR = 5;		// Invalid arguments provided to the API. This error can also indicate that the application was not correctly signed or properly set up for In-app Billing in Google Play, or does not have the necessary permissions in its manifest
	private static final int BILLING_RESPONSE_RESULT_ERROR = 6;					// Fatal error during the API action
	private static final int BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED = 7;	// Failure to purchase since item is already owned
	private static final int BILLING_RESPONSE_RESULT_ITEM_NOT_OWNED = 8;		// Failure to consume since item is not owned
	
	public InAppPurchasingService(Activity parent, PurchasingServiceListener listener){
		this.parent = parent;
		this.listener = listener;
	}
	
	public Activity getActivity(){
		return this.parent;
	}
	
	/**
	 * Creates service connection
	 */
	public void createConnection(){
		
		this.serviceConnection = new ServiceConnection() {
			
			   @Override
			   public void onServiceDisconnected(ComponentName name) {
			       billingService = null;
			   }

			   @Override
			   public void onServiceConnected(ComponentName name, IBinder service) {
				   billingService = IInAppBillingService.Stub.asInterface(service);
				   InAppPurchasingService.this.listener.onPurchasingServiceConnected();
				   // test only
				   try {
					   MyLog.i("InAppPurchasingService", "Calling getAvailableProducts via GooglePlay...");
					   InAppPurchasingService.this.getAvailableProducts();
				   } 
				   catch (GingerException e) {
					   e.printStackTrace();
				   }
				   // end of test
			   }
			};
	}
	
	/**
	 * Call this onCreate of parent Activity.
	 * @throws GingerException
	 */
	public void connect() throws GingerException {
	
		if(this.serviceConnection == null){
			throw new GingerException("Call create method first");
		}
		
		Intent googlePlaySvc = new Intent("com.android.vending.billing.InAppBillingService.BIND");		
		this.parent.bindService(googlePlaySvc, this.serviceConnection, Context.BIND_AUTO_CREATE);
	}
	
	/**
	 * Call this onDestroy of parent Activity
	 * @throws GingerException 
	 */
	public void disconnect() throws GingerException{
		
		if(this.serviceConnection == null){
			throw new GingerException("Call create method first");
		}
		
		this.parent.unbindService(this.serviceConnection);
	}
	
	/**
	 * Checks whether the service binding is done properly
	 * @return
	 */
	public boolean isConnected(){
		
		if(this.serviceConnection == null){
			return false;
		}
		
		if(this.billingService == null){
			return false;
		}
				
		return true;
	}
	
	/**
	 * Make sure that you call this method asynchronously. Otherwise, it might block your main thread.
	 * @param purchaseItemSku
	 * @param developerPayload
	 * @return
	 * @throws GingerException
	 */
	public boolean startBuyingItem(String purchaseItemSku, String developerPayload) throws GingerException{
		
		if(!this.isConnected()){
			throw new GingerException("Application hasn't connected to the Google Play Billing service yet.");
		}
		
		Bundle buyIntentBundle = null;
		try {
			buyIntentBundle = this.billingService.getBuyIntent(
					BILLING_API_VERSION,
					this.parent.getPackageName(),
					purchaseItemSku, 
					InAppPurchasingService.INAPP_PURCHASE, 
					developerPayload);
		} 
		catch (RemoteException e) {			
			String errMsg = "Purchasing credit couldn't be placed";
			MyLog.e("InAppPurchasingService", errMsg + ": " + e.getMessage());
			throw new GingerException(errMsg, e);
		}
		
		int responseCode = buyIntentBundle.getInt("RESPONSE_CODE", -1);
		if(responseCode == BILLING_RESPONSE_RESULT_USER_CANCELED){
			MyLog.i("InAppPurchasingService", "User cancelled in-app purchase.");
			return false;
		}
		
		/*
		if(responseCode == BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED){
			
		}
		*/
		
		if(responseCode != BILLING_RESPONSE_RESULT_OK){
			String errMsg = "Purchasing credit failed: " + getMessageForErrorCode(responseCode);
			MyLog.e("InAppPurchasingService", errMsg);
			throw new GingerException(errMsg);	
		}
		
		PendingIntent pendingIntent = buyIntentBundle.getParcelable("BUY_INTENT");
		
		try {
			// this goes to onActivityResult of parent Activity
			// You need to check 2 things: Activity.RESULT_OK, Activity.RESULT_CANCELED
			// See processActivityResult method below
			this.parent.startIntentSenderForResult(
					pendingIntent.getIntentSender(),
					INAPP_PURCHASE_REQUEST_CODE, 
					new Intent(), 
					0, 
					0,
					0);
		} 
		catch (SendIntentException e) {
			String errMsg = "Purchasing credit couldn't be completed";
			MyLog.e("InAppPurchasingService", errMsg + ": " + e.getMessage());
			throw new GingerException(errMsg, e);
		}
		
		return true;
	}
	
	/**
	 * Make sure that you call this method asynchronously. Otherwise, it might block your main thread.
	 * @param purchaseToken
	 * @throws GingerException
	 */
	public void consumePurchaseToEnableReBuy(String purchaseToken) throws GingerException{
		
		if(!this.isConnected()){
			throw new GingerException("Application hasn't connected to the Google Play Billing service yet.");
		}
		
		int responseCode = -1;
		try {
			responseCode = this.billingService.consumePurchase(BILLING_API_VERSION, this.parent.getPackageName(), purchaseToken);
		} 
		catch (RemoteException e) {
			String errMsg = "Consuming purchase token failed";
			MyLog.e("InAppPurchasingService", errMsg + ": " + e.getMessage());
			throw new GingerException(errMsg, e);
		}
		
		if(responseCode != BILLING_RESPONSE_RESULT_OK){
			String errMsg = "Consuming purchase token failed: " + getMessageForErrorCode(responseCode);
			MyLog.e("InAppPurchasingService", errMsg);
			throw new GingerException(errMsg);	
		}
	}
	
	/**
	 * This is for testing purposes only
	 * @throws GingerException 
	 */
	public String getAvailableProducts() throws GingerException{
		
		if(!this.isConnected()){
			throw new GingerException("Application hasn't connected to the Google Play Billing service yet.");
		}
		
		ArrayList<String> skuList = new ArrayList<String> ();
		skuList.add("buy_30_credit_test_version_01");
		skuList.add("buy_50_credit_test_version_01");
		skuList.add("buy_100_credit_test_version_01");
		skuList.add("buy_200_credit_test_version_01");
		skuList.add("buy_300_credit_test_version_01");
		skuList.add("buy_500_credit_test_version_01");
		skuList.add("buy_1000_credit_test_version_01");
		
		Bundle querySkus = new Bundle();
		querySkus.putStringArrayList("ITEM_ID_LIST", skuList);
		
		Bundle skuDetails = null;
		try {
			MyLog.i("InAppPurchasingService", "Getting available in-app SKUs");
			skuDetails = this.billingService.getSkuDetails(
					BILLING_API_VERSION,
					this.parent.getPackageName(), 
					INAPP_PURCHASE, 
					querySkus);
		} 
		catch (RemoteException e) {
			String errMsg = "Retrieving available products failed";
			MyLog.e("InAppPurchasingService", errMsg + ": " + e.getMessage());
			throw new GingerException(errMsg, e);
		}
		
		int responseCode = skuDetails.getInt("RESPONSE_CODE", -1);
		if(responseCode != BILLING_RESPONSE_RESULT_OK){
			String errMsg = "Retrieving available products failed: " + getMessageForErrorCode(responseCode);
			MyLog.e("InAppPurchasingService", errMsg);
			throw new GingerException(errMsg);	
		}
		
		String all = "";
		ArrayList<String> products = new ArrayList<String>(); 
		ArrayList<String> responseList = skuDetails.getStringArrayList("DETAILS_LIST");
		MyLog.i("InAppPurchasingService", "ResponseList count: " + responseList.size());
		for (String thisResponse : responseList) {
			try {
				JSONObject object = new JSONObject(thisResponse);
				String sku = object.getString("productId");
				String price = object.getString("price");
				String pro = sku + " - " + price;
				products.add(pro);
				all += pro + ", ";
				MyLog.i("InAppPurchasingService", pro);
			} 
			catch (JSONException e) {
				e.printStackTrace();
				MyLog.e("InAppPurchasingService", "JSONObject error: " + e.getMessage());
			}
		}
		
		return all;
	}
	
	/**
	 * Process activity result after purchase.
	 * Call this from YourActivity.onActivityResult. 
	 * Check requestCode == INAPP_PURCHASE_REQUEST_CODE && resultCode == Activity.RESULT_OK before calling this method.
	 * Also check responseCode == BILLING_RESPONSE_RESULT_OK before calling this method. Otherwise you will get exception. 
	 * @param requestCode
	 * @param resultCode
	 * @param data
	 * @return
	 * @throws GingerException
	 */
	public InAppPurchaseResult processActivityResult(int requestCode, int resultCode, Intent data) throws GingerException{
		
		if(requestCode != INAPP_PURCHASE_REQUEST_CODE){
			throw new GingerException("Unexpected failure when processing credit purchase. Request Code is wrong: " + requestCode);
		}
		
		if (resultCode != Activity.RESULT_OK) {
			throw new GingerException("Unexpected failure when processing credit purchase. Result Code is not a success: " + resultCode);
		}
		
		if(data == null){
			throw new GingerException("Unexpected failure when processing credit purchase. Returned intent data is null.");
		}
		
		int responseCode = data.getIntExtra("RESPONSE_CODE", 0);
		if(responseCode != BILLING_RESPONSE_RESULT_OK){
			throw new GingerException("Unexpected failure when processing credit purchase. Intent data has error code: " + responseCode);
		}
		
		//String dataSignature = data.getStringExtra("INAPP_DATA_SIGNATURE");
		String purchaseData = data.getStringExtra("INAPP_PURCHASE_DATA");	
		MyLog.i("InAppPurchasingService", "INAPP_PURCHASE_DATA: " + purchaseData);
		InAppPurchaseResult purchaseResult = InAppPurchaseResult.createFromJsonString(purchaseData);		
		if(purchaseResult.getPurchaseState().getIntValue() != InAppPurchaseState.Purchased.getIntValue()){
			throw new GingerException("Unexpected purchase result state: " + purchaseResult.getPurchaseState().getIntValue());
		}
		
		return purchaseResult;
	}
	
	/**
	 * Converts Google Play Billing Service error codes to human-readable error messages
	 * @param errorCode
	 * @return
	 */
	private static String getMessageForErrorCode(int errorCode){
		
		if(errorCode == BILLING_RESPONSE_RESULT_OK){
			return "Success";
		}
		else if(errorCode == BILLING_RESPONSE_RESULT_USER_CANCELED){
			return "User pressed back or canceled a dialog";
		}
		else if(errorCode == BILLING_RESPONSE_RESULT_BILLING_UNAVAILABLE){
			return "Billing API version is not supported for the type requested";
		}
		else if(errorCode == BILLING_RESPONSE_RESULT_ITEM_UNAVAILABLE){
			return "Requested product is not available for purchase";
		}
		else if(errorCode == BILLING_RESPONSE_RESULT_DEVELOPER_ERROR){
			// Invalid arguments provided to the API
			// This error can also indicate that the application was not correctly signed 
			// or properly set up for In-app Billing in Google Play, 
			// or does not have the necessary permissions in its manifest
			return "Invalid In-App purchase arguments or invalid configuration";
		}
		else if(errorCode == BILLING_RESPONSE_RESULT_ERROR){
			return "Fatal error during the API action";
		}
		else if(errorCode == BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED){
			return "Failure to purchase since item is already owned";
		}
		else if(errorCode == BILLING_RESPONSE_RESULT_ITEM_NOT_OWNED){
			return "Failure to consume since item is not owned";
		}
		else {
			return "Unknown error (Code = " + errorCode + ")";
		}		
	}
}
