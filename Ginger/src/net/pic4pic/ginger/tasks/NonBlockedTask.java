package net.pic4pic.ginger.tasks;

import net.pic4pic.ginger.utils.MyLog;
import android.os.AsyncTask;

// usage:
// NonBlockedTask.Run(new ITask(){ Service.getInstance().blah(); });
public class NonBlockedTask {
	
	/**
	 * This is good to send data to server at background
	 * @param task
	 */
	public static void SafeRun(final ITask task){
		
		// create background task
		AsyncTask<String, Void, Void> backgroundTask = new AsyncTask<String, Void, Void>(){
			@Override
			protected Void doInBackground(String... params) {
				try
				{
					task.perform();
				}
				catch(Exception e)
				{
					MyLog.e("BackgroundTask", "Background task failed: " + e.getMessage());
				}
				
				return null;
			}
		}; 
		
		// execute
		backgroundTask.execute();
	}
	
	/**
	 * This is good for UI effects
	 * @param sleepMilliSeconds
	 * @param task
	 */
	public static void SafeSleepAndRunOnUI(final int sleepMilliSeconds, final ITask task){
		
		// create background task
		AsyncTask<String, Void, Void> backgroundTask = new AsyncTask<String, Void, Void>(){
			
			/**
			 * This part gets executed on the UI
			 */
			@Override
			protected Void doInBackground(String... params) {
				try
				{
					Thread.sleep(sleepMilliSeconds);
				}
				catch(InterruptedException e)
				{
					MyLog.e("BackgroundTask", "Sleep interrupted: " + e.getMessage());
				}
				
				return null;
			}

			/* 
			 * This part gets executed on the UI Thread
			 */
			@Override
			protected void onPostExecute(Void result) {				
				task.perform();				
				super.onPostExecute(result);
			}
		}; 
		
		// execute
		backgroundTask.execute();
	}	
}
