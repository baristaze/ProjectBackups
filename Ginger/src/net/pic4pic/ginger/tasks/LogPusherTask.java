package net.pic4pic.ginger.tasks;

import java.util.ArrayList;

import android.os.AsyncTask;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.ClientLogRequest;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.LogBag;

public class LogPusherTask extends AsyncTask<String, Void, Integer> {
	
	private final int BulkSize = 50;
	
	private int initialCount;
	private ArrayList<LogBag> logs;	
	public LogPusherTask(ArrayList<LogBag> logs){
		 this.logs = logs;
		 this.initialCount = this.logs.size();
	}	 

	@Override
	protected Integer doInBackground(String... params) {
		
		int sentSoFar = 0;
		
		int size = 1;	
		while(size > 0){
			size = Math.min(logs.size(), BulkSize);
			if(size > 0){
				ArrayList<LogBag> subList = new ArrayList<LogBag>();
				for(int x=0; x<size; x++){
					subList.add(this.logs.remove(0)); // always remove 0th element
				}
				
				ClientLogRequest request = new ClientLogRequest();
				request.addRange(subList);
				
				try {
					BaseResponse response = Service.getInstance().sendClientLogs(request);
					if(response.getErrorCode() == 0){
						sentSoFar += subList.size();
					}
				} 
				catch (Exception e) {
					// this is intentionally not MyLog
					android.util.Log.e("Ginger-LogPusherTask", "Sending client logs to server failed. Log Count: " + subList.size() + ", Exception: " + e.toString());
				}
			}
		}
			 
		return sentSoFar;	
	}
	
	 @Override
	 protected void onPostExecute(Integer result) {
		// this is intentionally not MyLog
		 android.util.Log.i("Ginger-LogPusherTask", result + " out of " + this.initialCount + " logs have been sent.");
	 }
}
