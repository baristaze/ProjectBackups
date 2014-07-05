package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

import android.app.Activity;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Build;

import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.PushNotificationHelpers;

public class MobileDevice extends BaseRequest {
	
	@SerializedName("AppVersion")
	protected String appVersion;
	
	@SerializedName("OSVersion")
	protected String osVersion;
	
	@SerializedName("SDKVersion")
	protected String sdkVersion;
	
	@SerializedName("DeviceType")
	protected String deviceType;
	
	@SerializedName("PushNotifRegId")
	protected String pushNotifRegId;
	
	public static MobileDevice getInstance(Activity activity){
		return MobileDevice.getInstance(activity, PushNotificationHelpers.getRegistrationId(activity));
	}
	
	public static MobileDevice getInstance(Activity activity, String pushNotifRegId){
		
		MobileDevice device = new MobileDevice();
		device.appVersion = "" + MobileDevice.deriveAppVersion(activity);
		device.osVersion = MobileDevice.deriveOSVersion();
		device.sdkVersion = Integer.toString(MobileDevice.deriveSDKVersion());
		device.deviceType = MobileDevice.deriveDeviceName();
		device.pushNotifRegId = pushNotifRegId;
		
		return device;
	}
	
	/**
	 * @return the appVersion
	 */
	public String getAppVersion() {
		return appVersion;
	}

	/**
	 * @param appVersion the appVersion to set
	 */
	public void setAppVersion(String appVersion) {
		this.appVersion = appVersion;
	}

	/**
	 * @return the osVersion
	 */
	public String getOsVersion() {
		return osVersion;
	}

	/**
	 * @param osVersion the osVersion to set
	 */
	public void setOsVersion(String osVersion) {
		this.osVersion = osVersion;
	}

	/**
	 * @return the sdkVersion
	 */
	public String getSdkVersion() {
		return sdkVersion;
	}

	/**
	 * @param sdkVersion the sdkVersion to set
	 */
	public void setSdkVersion(String sdkVersion) {
		this.sdkVersion = sdkVersion;
	}

	/**
	 * @return the deviceType
	 */
	public String getDeviceType() {
		return deviceType;
	}

	/**
	 * @param deviceType the deviceType to set
	 */
	public void setDeviceType(String deviceType) {
		this.deviceType = deviceType;
	}

	/**
	 * @return the pushNotifRegId
	 */
	public String getPushNotifRegId() {
		return pushNotifRegId;
	}

	/**
	 * @param pushNotifRegId the pushNotifRegId to set
	 */
	public void setPushNotifRegId(String pushNotifRegId) {
		this.pushNotifRegId = pushNotifRegId;
	}

	public static int deriveAppVersion(Activity activity){	
		try {
	        PackageInfo packageInfo = activity.getPackageManager().getPackageInfo(activity.getPackageName(), 0);
	        return packageInfo.versionCode;
	    } 
	    catch (NameNotFoundException e) {
	        // should never happen
	    	MyLog.e("PushNotificationHelpers", "Could not get package name: " + e);
	    	return 0;
	    }
	}
	
	public static String deriveOSVersion(){
		return android.os.Build.VERSION.RELEASE;
	}
	
	public static int deriveSDKVersion(){
		return android.os.Build.VERSION.SDK_INT;
	}
	
	public static String deriveDeviceName() {
		
		String manufacturer = Build.MANUFACTURER;
		String model = Build.MODEL;
		if (model.startsWith(manufacturer)) {
			return MobileDevice.capitalizeDeviceName(model);
		} 
		else {
			return MobileDevice.capitalizeDeviceName(manufacturer) + " " + model;
		}
	}

	private static String capitalizeDeviceName(String s) {
		
		if (s == null || s.length() == 0) {
			return "";
		}
		
		char first = s.charAt(0);
		if (Character.isUpperCase(first)) {
			return s;
		} 
		else {
			return Character.toUpperCase(first) + s.substring(1);
		}
	}
}
