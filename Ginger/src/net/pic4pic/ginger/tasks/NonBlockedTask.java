package net.pic4pic.ginger.tasks;

import net.pic4pic.ginger.utils.MyLog;
import android.os.AsyncTask;

// usage:
// NonBlockedTask.Run(new ITask(){ Service.getInstance().blah(); });
public class NonBlockedTask {
	
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
}
