package net.pic4pic.ginger.tasks;

import android.app.Activity;
import android.os.AsyncTask;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.MobileDevice;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class TrackDeviceTask extends AsyncTask<String, Void, MobileDevice> {

	private Activity activity;
	
	public TrackDeviceTask(Activity activity){		
		this.activity = activity;
	}
	
	@Override
	protected MobileDevice doInBackground(String... urls) {
		 
		MobileDevice mobileDevice = MobileDevice.getInstance(this.activity);
			 
		try {
			MyLog.i("TrackDeviceTask", "Sending device info to the server...");
			BaseResponse response = Service.getInstance().trackDevice(this.activity, mobileDevice);
			if(response.getErrorCode() != 0){
				MyLog.e("TrackDeviceTask", "Sending device info to the server failed: " + response.getErrorMessage());
			}			 
		} 
		catch (GingerException e) {
			e.printStackTrace();
			MyLog.e("TrackDeviceTask", "Sending device info to the server: " + e);
		}
			 
		return mobileDevice;
	}
	 
	 @Override
	 protected void onPostExecute(MobileDevice result) {
		 // do nothing
	 }
}
