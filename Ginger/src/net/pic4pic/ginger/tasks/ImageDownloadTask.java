package net.pic4pic.ginger.tasks;

import java.io.InputStream;
import java.util.UUID;

import net.pic4pic.ginger.utils.ImageCacher;
import net.pic4pic.ginger.utils.MyLog;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.widget.ImageView;

public class ImageDownloadTask extends AsyncTask<String, Void, Bitmap> {
	
	private UUID imageId;
	private ImageView imageView;
	private boolean clickableAfterDownload;
	
	public ImageDownloadTask(UUID imageId, ImageView imageView){
		this(imageId, imageView, false);
	}
		
    public ImageDownloadTask(UUID imageId, ImageView imageView, boolean clickableAfterDownload) {
        this.imageId = imageId;
    	this.imageView = imageView;
        this.clickableAfterDownload = clickableAfterDownload;
        
        if(this.imageId == null){
        	this.imageId = new UUID(0, 0);
        	MyLog.w("ImageDownloadTask", "Image ID hasn't been defined: " + this.imageId );
        }
    }

    @Override
    protected Bitmap doInBackground(String... urls) {   
    	
    	// check cache first
    	if(ImageCacher.Instance().exists(this.imageId)){
    		// MyLog.i("ImageDownloadTask", "Cached image is used: " + this.imageId.toString());
    		return ImageCacher.Instance().get(this.imageId); 
    	}
    	
    	// else... download it
    	Bitmap bitmap = null;    	
    	if(urls != null && urls.length > 0){
	        String urldisplay = urls[0];	        
	        try {
	        	MyLog.v("ImageDownloadTask", "Downloading Image... " + this.imageId.toString());
	            InputStream in = new java.net.URL(urldisplay).openStream();
	            bitmap = BitmapFactory.decodeStream(in);
	        } 
	        catch (Exception e) {
	            MyLog.e("Error", "Image download failed with error: " + e.getMessage());
	            e.printStackTrace();
	        }
    	}
    	else{
    		MyLog.e("ImageDownloadTask", "ImageDownloadTask.execute() method should have a valid Url to download.");
    	}
        return bitmap;
    }
    
    @Override
    protected void onPostExecute(Bitmap result) {
    	
    	if(result != null){    		
    		
    		// set it to the cache
    		ImageCacher.Instance().put(this.imageId, result);
    		
    		// update resources
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
