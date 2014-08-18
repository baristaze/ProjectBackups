package net.pic4pic.ginger.utils;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;

import net.pic4pic.ginger.R;
import net.pic4pic.ginger.entities.MobileDevice;

public class PushNotificationHelpers {

	private static final String PROPERTY_REG_ID = "registration_id";
    private static final String PROPERTY_APP_VERSION = "appVersion";
    
	public static final String SENDER_ID = "250040200694";
	
	private final static int PLAY_SERVICES_RESOLUTION_REQUEST = 9000;
	
	/**
	 * Check the device to make sure it has the Google Play Services APK. If
	 * it doesn't, display a dialog that allows users to download the APK from
	 * the Google Play Store or enable it in the device's system settings.
	 */
	public static boolean checkPlayServices(Activity activity) {
		
	    int resultCode = GooglePlayServicesUtil.isGooglePlayServicesAvailable(activity);
	    if (resultCode == ConnectionResult.SUCCESS) {
	    	MyLog.bag().v("PushNotificationHelpers", "Google Play Services is available.");
	    	return true;
	    }
	    
	    if (GooglePlayServicesUtil.isUserRecoverableError(resultCode)) {
	    	MyLog.bag().i("PushNotificationHelpers", "Google Play Services is NOT available but error is recovarable.");
            GooglePlayServicesUtil.getErrorDialog(resultCode, activity, PLAY_SERVICES_RESOLUTION_REQUEST).show();
        } 
        else {
        	
        	MyLog.bag().e("PushNotificationHelpers", "This device does not support Google Play Services. Push Notifications won't be received.");
        	
        	GingerHelpers.showErrorMessageIfNotSupressed(
        			activity, 
        			R.string.google_play_svc_not_available, 
        			R.string.pref_error_hide_google_play_not_available);
        }
        
        return false;
	}
	
	/**
	 * Stores the registration ID and application versionCode in the application's
	 * {@code SharedPreferences}.
	 *
	 * @param context application's context.
	 * @param regId registration ID
	 */
	public static void storeRegistrationId(Activity activity, String registrationId) {
		
		SharedPreferences prefs = activity.getSharedPreferences(
				activity.getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
	    int appVersion = MobileDevice.deriveAppVersion(activity);
	    
	    MyLog.bag().i("PushNotificationHelpers", "Saving push notification registrationId on app version " + appVersion);
	    SharedPreferences.Editor editor = prefs.edit();
	    editor.putString(PROPERTY_REG_ID, registrationId);
	    editor.putInt(PROPERTY_APP_VERSION, appVersion);
	    editor.commit();
	}
	
	/**
	 * Gets the current registration ID for application on GCM service.
	 * <p>
	 * If result is empty, the app needs to register.
	 *
	 * @return registration ID, or empty string if there is no existing
	 *         registration ID.
	 */
	public static String getRegistrationId(Activity activity) {
		
		SharedPreferences prefs = activity.getSharedPreferences(
				activity.getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
	    String registrationId = prefs.getString(PROPERTY_REG_ID, "");
	    if (registrationId.isEmpty()) {
	        MyLog.bag().i("PushNotificationHelpers", "Push notification registration is not found.");
	        return "";
	    }
	    
	    // Check if application was updated; if so, it must clear the registration ID
	    // since the existing regID is not guaranteed to work with the new application version
	    int registeredVersion = prefs.getInt(PROPERTY_APP_VERSION, Integer.MIN_VALUE);
	    int currentVersion = MobileDevice.deriveAppVersion(activity);
	    if (registeredVersion != currentVersion) {
	        MyLog.bag().i("PushNotificationHelpers", "App version has changed. We need to refresh the push notification registration.");
	        return "";
	    }
	    
	    return registrationId;
	}
}
