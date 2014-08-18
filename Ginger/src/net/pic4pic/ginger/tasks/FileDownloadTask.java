package net.pic4pic.ginger.tasks;

import java.io.InputStream;

import android.os.AsyncTask;

import net.pic4pic.ginger.utils.MyLog;

// second parameter to the generic base is about onProgressUpdate(), which is a base
public class FileDownloadTask extends AsyncTask<String, Integer, InputStream> {
	
	// interface for listener
	public interface FileDownloadListener{
		public void onFileDownloadComplete(InputStream file);
	}
	
	// fields and constructor
	private FileDownloadListener listener;
	public FileDownloadTask(FileDownloadListener listener){
		this.listener = listener;
	}
	
	@Override
    protected InputStream doInBackground(String... urls) {
		InputStream file = null;
    	if(urls != null && urls.length > 0){
	        String urldisplay = urls[0];	        
	        try {
	        	file = new java.net.URL(urldisplay).openStream();
	        } 
	        catch (Exception e) {
	            MyLog.bag().e("Error", e.getMessage());
	            e.printStackTrace();
	        }
    	}
    	else{
    		MyLog.bag().e("FileDownloadTask", "FileDownloadTask.execute() method should have a valid Url to download.");
    	}
        return file;
    }
	
	@Override
	protected void onPostExecute(InputStream file) {
		if(this.listener != null){
			this.listener.onFileDownloadComplete(file);
		}
    }
}
