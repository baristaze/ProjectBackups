package net.pic4pic.ginger.tasks;

import android.content.Context;

import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class ImageUploadTask extends BlockedTask<String, Void, ImageUploadResponse> {

	private ImageUploadRequest request;
	private ImageUploadListener listener;
	
	public ImageUploadTask(ImageUploadListener listener, Context context, ImageUploadRequest request){
		
		super(context);
		this.listener = listener;
		this.request = request;
	}
	
	@Override
    protected ImageUploadResponse doInBackground(String... execArgs) {
		
		try {
			return Service.getInstance().uploadProfileImage(this.context, this.request);
		} 
		catch (GingerException e) {
			
			MyLog.e("ImageUploadTask", e.toString());
			
			ImageUploadResponse response = new ImageUploadResponse();
			response.setErrorCode(1);
			response.setErrorMessage(e.getMessage());
			return response;
		}
		catch(Exception e){
			
			MyLog.e("ImageUploadTask", e.toString());
			
			ImageUploadResponse response = new ImageUploadResponse();
			response.setErrorCode(1);
			response.setErrorMessage("Connection to server failed when uploading picture. Please try again!");
			return response;	
		}
	}
		
    protected void onPostExecute(ImageUploadResponse response) {    	
    	super.onPostExecute(response);
    	this.listener.onUpload(this.request, response);
    }
	
	public interface ImageUploadListener{    	
    	public void onUpload(ImageUploadRequest request, ImageUploadResponse response);
    }
}
