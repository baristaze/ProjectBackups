package net.pic4pic.ginger.tasks;

import java.io.IOException;

import com.google.android.gms.gcm.GoogleCloudMessaging;

import android.app.Activity;
import android.os.AsyncTask;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.MobileDevice;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.PushNotificationHelpers;

public class PushNotificationRegisterTask extends AsyncTask<String, Void, String> {
	
	private Activity activity;
	private String senderId;
	private GoogleCloudMessaging googleCloudMessaging;
	
	public PushNotificationRegisterTask(Activity activity, GoogleCloudMessaging googleCloudMessaging, String senderId){
		
		this.activity = activity;
		this.senderId = senderId;
		this.googleCloudMessaging = googleCloudMessaging;
	}
	
	@Override
	protected String doInBackground(String... urls) {
		
		String registrationId = "";
		try {
	        // register with Google Cloud Messaging Services
	        registrationId = this.googleCloudMessaging.register(this.senderId);
	        String logMessage = "Device is registered for push notifications with Google Play. Registration ID=" + registrationId;
	        MyLog.i("PushNotificationRegisterTask", logMessage);
        } 
        catch (IOException e) {
        	e.printStackTrace();
        	MyLog.e("PushNotificationRegisterTask", "Registration to the push notification with Google Play failed: " + e);
        }
		
		if(registrationId == null || registrationId.isEmpty()){
			return null;
		}
		 
		boolean savedSuccessfully = false;
		try {
			MyLog.i("PushNotificationRegisterTask", "Sending push notification registration ID to the server...");
			MobileDevice mobileDevice = MobileDevice.getInstance(this.activity, registrationId);
			BaseResponse response = Service.getInstance().trackDevice(this.activity, mobileDevice);
			if(response.getErrorCode() == 0){
				MyLog.i("PushNotificationRegisterTask", "Push notification registration ID has been sent to the server: " + registrationId);
				savedSuccessfully = true; 
			}
			else{
				MyLog.e("PushNotificationRegisterTask", "Push notification registration ID couldn't be sent to the server: " + response.getErrorMessage());
			}			 
		} 
		catch (GingerException e) {
			e.printStackTrace();
			MyLog.e("PushNotificationRegisterTask", "Push notification registration ID couldn't be sent to the server: " + e);
		}

         // Persist the regID - no need to register again.
		 if(savedSuccessfully){
			 PushNotificationHelpers.storeRegistrationId(this.activity, registrationId);
		 }
		 
         return registrationId;
	}
	 
	@Override
	protected void onPostExecute(String result) {
		// do nothing
	}
}
