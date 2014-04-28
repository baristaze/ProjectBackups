package net.pic4pic.ginger.tasks;

import java.io.InputStream;

import net.pic4pic.ginger.utils.MyLog;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.widget.ImageView;

public class ImageDownloadTask extends AsyncTask<String, Void, Bitmap> {
	
	private ImageView imageView;
	private boolean clickableAfterDownload;
	
	public ImageDownloadTask(ImageView imageView){
		this(imageView, false);
	}
	
    public ImageDownloadTask(ImageView imageView, boolean clickableAfterDownload) {
        this.imageView = imageView;
        this.clickableAfterDownload = clickableAfterDownload;
    }

    @Override
    protected Bitmap doInBackground(String... urls) {    	
    	Bitmap bitmap = null;    	
    	if(urls != null && urls.length > 0){
	        String urldisplay = urls[0];	        
	        try {
	            InputStream in = new java.net.URL(urldisplay).openStream();
	            bitmap = BitmapFactory.decodeStream(in);
	        } 
	        catch (Exception e) {
	            MyLog.e("Error", e.getMessage());
	            e.printStackTrace();
	        }
    	}
    	else{
    		MyLog.e("ImageDownloadTask", "ImageDownloadTask.execute() method should have a valid Url to download.");
    	}
        return bitmap;
    }

    protected void onPostExecute(Bitmap result) {
    	if(result != null){
    		this.imageView.setImageBitmap(result);
    		if(this.clickableAfterDownload){
    			this.imageView.setClickable(true);
    		}
    	}
    	else{
    		MyLog.e("ImageDownloadTask", "Image couldn't be downloaded? Result bitmap is null in onPostExecute()");
    	}
    }	
}
